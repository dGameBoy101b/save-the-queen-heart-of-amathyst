using System;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour, IActionProvider, IStartTurn, IEndTurn
{
	#region Instances
	private static readonly HashSet<Move> _instances = new();
	public static IReadOnlyCollection<Move> Instances => _instances;

	public void RegisterInstance()
	{
		_instances.Add(this);
	}

	public void UnregisterInstance()
	{
		_instances.Remove(this);
	}
	#endregion

	#region Move Points
	[SerializeField]
	[Tooltip("The maximum number of squares this can move each turn")]
	[Min(0)]
	private int _maximum;
	public int Maximum
	{
		get => this._maximum;
		set => this._maximum = Math.Max(0, value);
	}

	public int Current { get; private set; } = 0;

	public void RefreshPoints()
	{
		this.Current = this.Maximum;
		this.ClearValidMoves();
	}

	public void ExhaustPoints()
	{
		this.Current = 0;
		this.ClearValidMoves();
	}
	#endregion

	#region Valid Moves
	private GridSnapper _gridSnapper;
	public GridSnapper GridSnapper
	{
		get
		{
			if (this._gridSnapper == null)
			{
				this._gridSnapper = this.GetComponent<GridSnapper>();
			}
			return this._gridSnapper;
		}
	}

	private Dictionary<Vector2Int, int> _validMoves = null;
	public IReadOnlyDictionary<Vector2Int, int> ValidMoves
	{
		get
		{
			if (!this.HasCalculatedValidMoves())
				this._validMoves = this.CalculateValidMoves();
			return this._validMoves;
		}
	}

	public bool HasCalculatedValidMoves() => this._validMoves != null;

	public void ClearValidMoves()
	{
		this._validMoves = null;
	}

	public int? DistanceTo(Vector2Int position, int distance_before)
	{
		if (!this.GridSnapper.Bounds.IsInBounds((Vector3Int)position))
			return null; //out of bounds
		if (GridSnapper.GetSnapperAtPosition((Vector3Int)position) != null)
			return null; //occuppied
		var terrain = TerrainTiles.GetTerrainAtPosition((Vector3Int)position);
		if (terrain == null || !terrain.IsPassable)
			return null; //impassable
		var distance = terrain.MoveCost + distance_before;
		if (distance > this.Current)
			return null; //unaffordable
		return distance;
	}

	static readonly Vector2Int[] DIRECTIONS = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

	private Dictionary<Vector2Int, int> CalculateValidMoves()
	{
		Dictionary<Vector2Int, int> valid_moves = new();
		Vector2Int current_position = (Vector2Int)this.GridSnapper.CurrentCellPosition;
		Queue<Vector2Int> to_explore = new(new Vector2Int[]
		{
			current_position
		});

		for (Vector2Int explore = to_explore.Dequeue(); to_explore.TryPeek(out _); explore = to_explore.Dequeue())
		{
			int explore_distance;
			if (!valid_moves.TryGetValue(explore, out explore_distance))
				explore_distance = 0;

			foreach (var direction in DIRECTIONS)
			{
				Vector2Int position = explore + direction;
				var distance = this.DistanceTo(position, explore_distance);
				if (distance == null)
					continue; //invalid move

				int old_distance;
				bool has_old = valid_moves.TryGetValue(position, out old_distance);
				if (has_old && old_distance < distance)
					continue; //already found better path
				
				valid_moves[position] = distance.Value;
				if (distance.Value <= this.Current)
					to_explore.Enqueue(position);
			}
		}
		Debug.Log("Calculated valid moves", this);
		return valid_moves;
	}
	#endregion

	#region Provide Actions
	public IEnumerable<Vector2Int> GetValidCells()
	{
		return this.ValidMoves.Keys;
	}

	public void PerformAction(Vector2Int destination)
	{
		if (this.ValidMoves == null)
			throw new InvalidOperationException("Valid moves must be calculated first");
		int distance;
		if (!this.ValidMoves.TryGetValue(destination, out distance))
			throw new ArgumentException($"Cannot move to {destination}");
		this.Current -= distance;
		Vector2Int current = (Vector2Int)this.GridSnapper.CurrentCellPosition;
		this.GridSnapper.CurrentCellPosition = (Vector3Int)destination;
		foreach (var instance in Instances)
			instance.ClearValidMoves();
		Debug.Log($"{this.name} moved {distance} squares: {current} -> {destination}", this);
	}
	#endregion

	#region Turn Messages
	public void StartTurn(Team.Teams team)
	{
		if (team != this.GetTeam())
			return;
		this.RefreshPoints();
	}

	public void EndTurn(Team.Teams team)
	{
		this.ExhaustPoints();
	}
	#endregion

	#region Unity Messages
	private void OnEnable()
	{
		this.RegisterInstance();
	}

	private void OnDisable()
	{
		this.UnregisterInstance();
	}
	#endregion

	#region Gizmos
	public void DrawValidMovesGizmo()
	{
		if (!this.HasCalculatedValidMoves())
			return;
		Gradient cost_gradient = new();
		cost_gradient.SetKeys(
			new GradientColorKey[] {
				new(Color.blue,0),
				new(Color.white,1)},
			new GradientAlphaKey[] {
				new(.5f, 0),
				new(.5f, 1)});
		var grid = this.GridSnapper.Bounds.Grid;
		foreach (var item in this.ValidMoves)
		{
			Gizmos.color = cost_gradient.Evaluate(item.Value / (float)this.Maximum);
			Gizmos.DrawCube(grid.CellToWorld((Vector3Int)item.Key), grid.cellSize);
		}
	}

	private void OnDrawGizmosSelected()
	{
		this.DrawValidMovesGizmo();
	}
	#endregion
}
