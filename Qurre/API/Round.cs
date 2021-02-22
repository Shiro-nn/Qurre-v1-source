using GameCore;
using Respawning;
using Respawning.NamingRules;
using System;
using System.Reflection;

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
        public static void Restart() => Map.Host.PlayerStats.Roundrestart();
        public static void Start() => CharacterClassManager.ForceRoundStart();
        public static void AddUnit(SpawnableTeamType team, string text)
        {
            UnitNamingRule unitNamingRule;
            if (UnitNamingRules.AllNamingRules.TryGetValue(team, out unitNamingRule))
            {
                unitNamingRule.AddCombination(text, team);
            }
        }
        public static void RenameUnit(SpawnableTeamType team, int id, string newName)
        {
            RespawnManager.Singleton.NamingManager.AllUnitNames[id] = new SyncUnit
            {
                SpawnableTeam = (byte)team,
                UnitName = newName
            };
        }
        public static void ForceTeamRespawn(bool isCI) => RespawnManager.Singleton.ForceSpawnTeam(isCI ? SpawnableTeamType.ChaosInsurgency : SpawnableTeamType.NineTailedFox);
        public static void InvokeStaticMethod(this Type type, string methodName, object[] param)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Public;
            MethodInfo info = type.GetMethod(methodName, flags);
            info?.Invoke(null, param);
        }
        private static Player host;
        public static Player Host
        {
            get
            {
                if (host == null || host.ReferenceHub == null) host = new Player(PlayerManager.localPlayer);
                return host;
            }
        }
    }
}