#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GridBounds : MonoBehaviour
{
	public Vector3Int MinBound;

	public Vector3Int MaxBound;

	public void ClampCell(in Vector3Int cell_pos)
	{
		cell_pos.Clamp(this.MinBound, this.MaxBound);
	}

	#region Gizmos
#if UNITY_EDITOR
	public void DrawGizmo(GridLayout grid)
	{
		if (grid == null)
			return;
		switch (grid.cellLayout)
		{
			case GridLayout.CellLayout.Rectangle:
				this.DrawRectangleGizmo(grid);
				break;
			default:
				Debug.LogWarning($"Unsupported cell layout: {grid.cellLayout}", this);
				break;
		}
	}

	private void DrawRectangleGizmo(GridLayout grid)
	{
		Vector3 min = grid.CellToWorld(this.MinBound);
		Vector3 max = grid.CellToWorld(this.MaxBound);
		Vector3 center = (min + max) / 2;
		Vector3 size = max - min;
		Gizmos.DrawWireCube(center, size);
	}

	private void OnDrawGizmos()
	{
		if (Selection.activeTransform == null || !Selection.activeTransform.IsChildOf(this.transform))
			return;
		this.DrawGizmo(this.GetComponentInParent<GridLayout>());
	}
#endif
#endregion
}
