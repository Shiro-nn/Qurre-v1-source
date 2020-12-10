using GameCore;
using Respawning;
using Respawning.NamingRules;
using System;
namespace Qurre.API
{
    public static class Round
    {
        public static TimeSpan ElapsedTime => RoundStart.RoundLenght;
        public static DateTime StartedTime => DateTime.Now - ElapsedTime;
        public static bool IsStarted => RoundSummary.RoundInProgress();
        public static bool Lock
        {
            get => RoundSummary.RoundLock;
            set => RoundSummary.RoundLock = value;
        }
        public static bool LobbyLock
		{
			get => RoundStart.LobbyLock;
			set => RoundStart.LobbyLock = value;
		}
		public static void Restart() => Map.Host.playerStats.Roundrestart();
        public static void Start() => CharacterClassManager.ForceRoundStart();
        public static void AddUnit(Team team, string text)
        {
            UnitNamingRule unitNamingRule;
            if (UnitNamingRules.AllNamingRules.TryGetValue((SpawnableTeamType)team, out unitNamingRule))
            {
                unitNamingRule.AddCombination(text, (SpawnableTeamType)team);
            }
        }
    }
}