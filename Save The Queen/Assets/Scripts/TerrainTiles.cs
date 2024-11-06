using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TerrainTiles : MonoBehaviour
{
	[Tooltip("Whether units can move through this terrain")]
	public bool IsPassable = true;

	[Tooltip("The number of points it costs a unit to move to each square of this terrain")]
	public int MoveCost = 1;

	private Tilemap tilemap;
	public Tilemap Tilemap
	{
		get
		{
			if (this.tilemap == null)
				this.tilemap = this.GetComponent<Tilemap>();
			return this.tilemap;
		}
	}

	#region Instances
	private static readonly HashSet<TerrainTiles> instances = new();
	public static IReadOnlyCollection<TerrainTiles> Instances => instances;

	public void RegisterInstance()
	{
		instances.Add(this);
	}

	public void UnregisterInstance()
	{
		instances.Remove(this);
		RemoveTerrainFromPositionsCache(this);
	}
	#endregion

	#region Positions Dictionary
	private static readonly Dictionary<Vector3Int, TerrainTiles> positions_cache = new();
	public static TerrainTiles GetTerrainAtPosition(Vector3Int cell_position)
	{
		if (positions_cache.ContainsKey(cell_position))
			return positions_cache[cell_position];

		foreach (var terrain in Instances)
			if (terrain.Tilemap.HasTile(cell_position))
			{
				positions_cache[cell_position] = terrain;
				return terrain;
			}
		return null;
	}

	public static int RemoveTerrainFromPositionsCache(TerrainTiles terrain)
	{
		HashSet<Vector3Int> to_remove = new();
		foreach (var item in positions_cache)
			if (item.Value == terrain)
				to_remove.Add(item.Key);
		foreach (var key in to_remove)
			positions_cache.Remove(key);
		return to_remove.Count;
	}

	public static void ClearPositionsCache()
	{
		positions_cache.Clear();
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
}