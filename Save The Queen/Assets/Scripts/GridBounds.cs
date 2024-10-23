#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(GridLayout))]
public class GridBounds : MonoBehaviour
{
	private GridLayout grid;
	public GridLayout Grid
	{
		get
		{
			if (this.grid == null)
				this.grid = this.GetComponent<GridLayout>();
			return this.grid;
		}
	}

	public Vector3Int MinBound;

	public Vector3Int MaxBound;

	public void ClampCell(in Vector3Int cell_pos)
	{
		cell_pos.Clamp(this.MinBound, this.MaxBound);
	}

	#region Gizmos
#if UNITY_EDITOR
	public void DrawGizmo()
	{
		if (grid == null)
			return;
		switch (this.Grid.cellLayout)
		{
			case GridLayout.CellLayout.Rectangle:
				this.DrawRectangleGizmo();
				break;
			default:
				Debug.LogWarning($"Unsupported cell layout: {this.Grid.cellLayout}", this);
				break;
		}
	}

	private void DrawRectangleGizmo()
	{
		Vector3 min = this.Grid.CellToWorld(this.MinBound);
		Vector3 max = this.Grid.CellToWorld(this.MaxBound);
		Vector3 center = (min + max) / 2;
		Vector3 size = max - min;
		Gizmos.DrawWireCube(center, size);
	}

	private void OnDrawGizmos()
	{
		if (Selection.activeTransform == null || !Selection.activeTransform.IsChildOf(this.transform))
			return;
		this.DrawGizmo();
	}
#endif
#endregion
}
