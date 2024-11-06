using System.Collections.Generic;
using UnityEngine;

public interface IActionProvider
{
	public IEnumerable<Vector2Int> GetValidCells();

	public void PerformAction(Vector2Int destination);
}
