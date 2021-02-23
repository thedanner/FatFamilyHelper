/*
  TODO:
- Remove L4D-specific references in the source since the plugin itself uses both indexes and game-specific team names.
*/

#define VERSION "0.0.2"
/*
Version history:
2021-02-23  0.0.2 - Minor metadata and comment tweaks.
2020-11-15  0.0.1 - Initial release.
*/

/* PrintToChat colors:
x01 = Default (White)
x02 = Team? (Blue/Red)
x03 = Light Green
x04 = Green (yellow/orange in L4D)
*/

#pragma semicolon 1

#include <sourcemod>
#include <sdktools>

// constants.sp
#define L4D_TEAM_UNASSIGNED 0
#define L4D_TEAM_SPECTATOR 1
#define L4D_TEAM_SURVIVORS 2
#define L4D_TEAM_INFECTED 3


public Plugin:myinfo = 
{
	name = "[L4D] Print Info",
	author = "The Danner",
	description = "Prints misc. player info. Meant to be consumed and parsed by other components that can execute a command via rcon.",
	version = VERSION,
	url = "https://github.com/thedanner/Left4DeadHelper"
}


public OnPluginStart()
{
	RegConsoleCmd(
		"sm_printinfo",
		Command_PrintInfo,
		"Prints player information.");
	
	CreateConVar(
		"l4d_printinfo_version",
		VERSION,
		"Print Info plugin version",
		FCVAR_SPONLY|FCVAR_NOTIFY|FCVAR_REPLICATED|FCVAR_DONTRECORD);
}

public Action Command_PrintInfo(client, args)
{
	char teamName[16];
	
	ReplyToCommand(client, "[PI] BEGIN");
	// Skip client 0 since it's rcon and will never be on a team.
	for (int i = 1; i <= MaxClients; i++)
	{
		if (IsClientInGame(i))
		{
			int team = GetClientTeam(i);
			GetTeamName(team, teamName, sizeof(teamName));
			
			if (team == L4D_TEAM_SPECTATOR
				|| team == L4D_TEAM_SURVIVORS
				|| team == L4D_TEAM_INFECTED)
			{
				// Output line is "[PI] %L<%i><%s>" in SourceMod string formatting.
				// https://wiki.alliedmods.net/Format_Class_Functions_(SourceMod_Scripting)
				// %L expands to 1<2><3><> where 1 is the player's name, 2 is the player's userid,
				// and 3 is the player's Steam ID. If the client index is 0, the string will be: Console<0><Console><Console>
				// The next <> is team #. SPECTATOR = 1, SURVIVORS = 2, INFECTED = 3
				// The final <> is team name (Survivors, Infected, Spectator maybe?)
				// So, that means, if elements are indexed by <> with the name being 0th:
				// [0]: player's name
				// [1]: player's userid
				// [2]: player's steamid
				// [3]: ???
				// [4]: player's team index
				// [5]: player's team name
				
				ReplyToCommand(
					client,
					"[PI] %L<%i><%s>",
					i,
					team,
					teamName
				);
			}
		}
	}
	
	ReplyToCommand(client, "[PI] END");
	
	return Plugin_Handled;
}
