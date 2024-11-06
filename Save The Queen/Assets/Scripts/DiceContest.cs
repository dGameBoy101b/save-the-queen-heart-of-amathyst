using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class DiceContest : SingletonMonoBehaviour<DiceContest>
{
	public event Action<Team.Teams> OnResult;

	// to be replaced with on screen physics based dice system
	public void RollDice() 
	{
		var winner = Team.Teams.None;
		bool is_tie = Random.value < 1f / 6;
		if (!is_tie)
			winner = Random.value < .5 ? Team.Teams.Red : Team.Teams.Blue;
		Debug.Log($"rolled dice: {winner}", this);
		this.OnResult(winner);
	}
}
