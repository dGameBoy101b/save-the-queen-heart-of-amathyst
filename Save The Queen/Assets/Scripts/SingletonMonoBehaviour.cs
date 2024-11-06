using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour
	where T : SingletonMonoBehaviour<T>
{
	public static T Instance { get; private set; } = null;

	public void RegisterSingleton()
	{
		if (Instance == this)
			return;
		if (Instance != null)
			Debug.LogWarning($"Overwitting {typeof(T).Name} singleton: {Instance}", this);
		Instance = (T)this;
	}

	public void UnregisterSingleton()
	{
		if (Instance != this)
			return;
		Instance = null;
	}

	protected void OnEnable()
	{
		this.RegisterSingleton();
	}

	protected void OnDisable()
	{
		this.UnregisterSingleton();
	}
}
