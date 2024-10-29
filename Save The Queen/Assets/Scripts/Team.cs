using UnityEngine;

[DisallowMultipleComponent]
public class Team : MonoBehaviour
{
	public enum Teams
	{
		None,
		Red,
		Blue
	}

	public Teams Current;
}
