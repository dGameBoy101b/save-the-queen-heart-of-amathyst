using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GridSnapper))]
public class Proficiency : MonoBehaviour, IStartTurn, IEndTurn
{
	#region Range
	[SerializeField]
	[Min(0)]
	private int _maximum = 20;
	public int Maximum
	{
		get => this._maximum;
		set => this._maximum = Mathf.Max(0, value);
	}

	private int _current = 0;
	public int Current
	{
		get => this._current;
		set
		{
			this._current = Mathf.Clamp(value, 0, this.Maximum);
			this.OnCurrentChange.Invoke(this.Current);
			this.OnProgressChange.Invoke(this.Progress);
		}
	}

	[Tooltip("Invoked with the new current proficiency points")]
	public UnityEvent<int> OnCurrentChange = new();

	public float Progress => Mathf.Clamp01(this.Current / (float)this.Maximum);

	[Tooltip("Invoked with the new current progress between 0 and 1")]
	public UnityEvent<float> OnProgressChange = new();
	#endregion

	#region Kill Award
	[Tooltip("The proficency points awarded when this unit kills")]
	public int KillAward = 2;

	public void AwardKill()
	{
		this.Current += this.KillAward;
	}
	#endregion

	#region Experience Award
	[Tooltip("The proficiency points awarded each turn this unit is outside its castle")]
	public int ExperienceAward = 1;

	private GridSnapper _grid;
	public GridSnapper Grid
	{
		get
		{
			if (this._grid == null)
				this._grid = this.GetComponent<GridSnapper>();
			return this._grid;
		}
	}

	public Team.Teams StartTerrainTileTeam { get; private set; }

	public TerrainTiles GetCurrentTerrainTile()
	{
		Vector3Int cell_position = this.Grid.Bounds.Grid.WorldToCell(this.transform.position);
		return TerrainTiles.GetTerrainAtPosition(cell_position);
	}

	public bool ShouldAwardExperience(Team.Teams start_terrain, Team.Teams end_terrain)
	{
		Team.Teams team = this.GetTeam();
		return start_terrain != team && end_terrain != team;
	}

	public bool TryAwardExperience(Team.Teams start_terrain, Team.Teams end_terrain)
	{
		bool should = this.ShouldAwardExperience(start_terrain, end_terrain);
		if (should)
			this.Current += this.ExperienceAward;
		return should;
	}
	#endregion

	#region Turn Messages
	public void StartTurn(Team.Teams team)
	{
		if (team == this.GetTeam())
			this.StartTerrainTileTeam = this.GetCurrentTerrainTile().GetTeam();
	}

	public void EndTurn(Team.Teams team)
	{
		if (team == this.GetTeam())
			this.TryAwardExperience(this.StartTerrainTileTeam, this.GetCurrentTerrainTile().GetTeam());
	}
	#endregion

	#region Unity Messages
	private void Start()
	{
		this.OnCurrentChange.Invoke(this.Current);
		this.OnProgressChange.Invoke(this.Progress);
	}
	#endregion
}
