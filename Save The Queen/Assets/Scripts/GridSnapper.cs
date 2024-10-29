using System.Collections;
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

	public void SnapToGrid()
	{
		if (this.Bounds == null)
			return;
		var position = this.transform.position;
		var cell = this.Bounds.Grid.WorldToCell(position);
		var snapped = this.Bounds.ClampCell(cell);
		var center_offset = this.Bounds.Grid.LocalToWorld(this.Bounds.Grid.GetLayoutCellCenter());
		var final = this.Bounds.Grid.CellToWorld(snapped) + center_offset;
		if (position == final)
			return;
		this.transform.position = final;
		Debug.Log($"Snapped {this.name} to {this.Bounds}: {position} -> {snapped} -> {final}", this);
	}

	private void Update()
	{
		this.SnapToGrid();
	}
}
