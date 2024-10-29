using UnityEngine;

public static class TeamGetters
{
	public static Team.Teams GetTeam(this GameObject game_object)
	{
		if (game_object == null)
			return Team.Teams.None;
		var component = game_object.GetComponent<Team>();
		return component == null ? Team.Teams.None : component.Current;
	}

	public static Team.Teams GetTeam(this Component component)
	{
		return component == null ? Team.Teams.None : component.gameObject.GetTeam();
	}
}
