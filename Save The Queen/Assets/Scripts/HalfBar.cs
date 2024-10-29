using UnityEngine;
using UnityEngine.Events;

public class HalfBar : MonoBehaviour
{
	public UnityEvent<float> OnHalve = new();

	public void Halve(float value)
	{
		var half = Mathf.Clamp(value / 2f, 0, .5f);
		this.OnHalve.Invoke(half);
	}
}
