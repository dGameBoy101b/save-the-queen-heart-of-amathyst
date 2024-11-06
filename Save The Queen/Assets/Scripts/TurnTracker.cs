using System.Collections.Generic;
using UnityEngine;

public class TurnTracker : SingletonMonoBehaviour<TurnTracker>
{
	[Tooltip("The team that gets the first turn")]
	public Team.Teams FirstTeam;

	public HashSet<IStartTurn> StartTurnListeners = new();

	public HashSet<IEndTurn> EndTurnListeners = new();

	private IEnumerable<T> GetAllComponents<T>()
	{
		foreach (var root in this.gameObject.scene.GetRootGameObjects())
			foreach (var component in root.GetComponentsInChildren<T>(true))
				yield return component;
	}

	private Team.Teams _currentTeam = Team.Teams.None;
	public Team.Teams CurrentTeam
	{
		get => this._currentTeam;
		set
		{
			var old = this._currentTeam;
			if (old == value)
				return;
			if (old != Team.Teams.None)
				foreach (var component in this.EndTurnListeners)
					if (component != null)
						component.EndTurn(old);
			this._currentTeam = Team.Teams.None;
			Debug.Log($"Ended turn for {old}", this);
			if (value != Team.Teams.None)
				foreach (var component in this.StartTurnListeners)
					if (component != null)
						component.StartTurn(value);
			this._currentTeam = value;
			Debug.Log($"Started turn for {value}", this);
		}
	}

	public Team.Teams NextTeam()
	{
		switch (this.CurrentTeam)
		{
			case Team.Teams.Blue:
				return Team.Teams.Red;
			case Team.Teams.Red:
				return Team.Teams.Blue;
			default:
				return this.FirstTeam;
		}
	}

	public void NextTurn()
	{
		this.CurrentTeam = this.NextTeam();
	}

	private void Start()
	{
		this.StartTurnListeners = new(this.GetAllComponents<IStartTurn>());
		this.EndTurnListeners = new(this.GetAllComponents<IEndTurn>());
	}
}
