using UnityEngine;
using UnityEngine.Events;

public class IntegerDisplay : MonoBehaviour
{
	public UnityEvent<string> OnTextConversion = new();

	public void ConvertToText(int value)
	{
		string text = value.ToString();
		this.OnTextConversion.Invoke(text);
	}
}
