using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TerrainTiles : MonoBehaviour
{
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
		instances.Add(this);
	}

	private void OnDisable()
	{
		instances.Remove(this);
		RemoveTerrainFromPositionsCache(this);
	}
	#endregion
}