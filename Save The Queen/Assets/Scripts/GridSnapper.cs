using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class GridSnapper : MonoBehaviour
{
	private GridBounds bounds;
	public GridBounds Bounds
	{
		get
		{
			if (this.bounds == null)
				this.bounds = this.GetComponentInParent<GridBounds>();
			return this.bounds;
		}
	}

	private static readonly Dictionary<Vector3Int, GridSnapper> _cellPositions = new();
	public static IReadOnlyDictionary<Vector3Int, GridSnapper> CellPositions => _cellPositions;

	public static GridSnapper GetSnapperAtPosition(Vector3Int position)
	{
		if (CellPositions.TryGetValue(position, out GridSnapper snapper))
			return snapper;
		return null;
	}

	public Vector3Int? OldCellPosition { get; private set; } = null;
	public Vector3Int CurrentCellPosition
	{
		get
		{
			var position = this.transform.position;
			var cell = this.Bounds.Grid.WorldToCell(position);
			var snapped = this.Bounds.ClampCell(cell);
			this.UpdateCellPositions(snapped);
			return snapped;
		}
		set
		{
			value = this.Bounds.ClampCell(value);
			if (value == this.OldCellPosition)
				return;
			this.UpdateCellPositions(value);
			var center_offset = this.Bounds.Grid.LocalToWorld(this.Bounds.Grid.GetLayoutCellCenter());
			var final = this.Bounds.Grid.CellToWorld(value) + center_offset;
			this.transform.position = final;
			Debug.Log($"{this.name} snapped to {value}", this);
		}
	}

	private void UpdateCellPositions(Vector3Int position)
	{
		var old = this.OldCellPosition;
		if (position == old)
			return;
		if (old != null)
			_cellPositions.Remove(old.Value);
		if (!this.isActiveAndEnabled)
			return;
		this.OldCellPosition = position;
		if (_cellPositions.ContainsKey(position))
			Debug.LogWarning($"{this.name} is overwriting the position of {_cellPositions[position]?.name}", this);
		_cellPositions[position] = this;
		Debug.Log($"{this.name} changed position: {old} -> {position}", this);
	}

	public void SnapToGrid()
	{
		if (this.Bounds == null)
			return;
		this.CurrentCellPosition = this.CurrentCellPosition;
	}

	private void Update()
	{
		if (!Application.IsPlaying(this.gameObject))
			this.SnapToGrid();
	}

	private void OnEnable()
	{
		this.SnapToGrid();
	}

	private void OnDisable()
	{
		_cellPositions.Remove(this.CurrentCellPosition);
		this.OldCellPosition = null;
	}
}
