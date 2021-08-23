using GameCore;
using Qurre.API.Objects;
using Respawning;
using Respawning.NamingRules;
using System;
using UnityEngine;
namespace Qurre.API
{
    public static class Round
    {
        private static RespawnManager rm => RespawnManager.Singleton;
        private static RoundSummary rs => RoundSummary.singleton;
        internal static bool ForceEnd { get; set; } = false;
        public static TimeSpan ElapsedTime => RoundStart.RoundLength;
        public static DateTime StartedTime => DateTime.Now - ElapsedTime;
        public static int CurrentRound { get; internal set; } = 0;
        public static int ActiveGenerators { get; internal set; } = 0;
        public static float NextRespawn
        {
            get => rm._timeForNextSequence - (float)rm._stopwatch.Elapsed.TotalSeconds;
            set => rm._timeForNextSequence = value + (float)rm._stopwatch.Elapsed.TotalSeconds;
        }
        public static bool Started => RoundSummary.RoundInProgress();
        public static bool Ended => rs.RoundEnded;
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
        public static int EscapedDPersonnel
        {
            get => RoundSummary.escaped_ds;
            set => RoundSummary.escaped_ds = value;
        }
        public static int EscapedScientists
        {
            get => RoundSummary.escaped_scientists;
            set => RoundSummary.escaped_scientists = value;
        }
        public static int ScpKills
        {
            get => RoundSummary.kills_by_scp;
            set => RoundSummary.kills_by_scp = value;
        }
        public static void Restart() => Server.Host.PlayerStats.Roundrestart();
        public static void Start() => CharacterClassManager.ForceRoundStart();
        public static void End() => ForceEnd = true;
        public static void DimScreen() => rs.RpcDimScreen();
        public static void ShowRoundSummary(RoundSummary.SumInfo_ClassList remainingPlayers, LeadingTeam team)
        {
            var timeToRoundRestart = Mathf.Clamp(GameCore.ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);
            rs.RpcShowRoundSummary(rs.classlistStart, remainingPlayers, team, EscapedDPersonnel, EscapedScientists, ScpKills, timeToRoundRestart);
        }
        public static void AddUnit(TeamUnitType team, string unit)
        {
            UnitNamingRule unitNamingRule;
            if (!UnitNamingRules.AllNamingRules.TryGetValue((SpawnableTeamType)team, out unitNamingRule)) return;
            unitNamingRule.AddCombination(unit, (SpawnableTeamType)team);
        }
        public static void RenameUnit(TeamUnitType team, int id, string newName)
        {
            RespawnManager.Singleton.NamingManager.AllUnitNames[id] = new SyncUnit
            {
                SpawnableTeam = (byte)team,
                UnitName = newName
            };
        }
        public static void RemoveUnit(int id)
        {
            RespawnManager.Singleton.NamingManager.AllUnitNames.RemoveAt(id);
        }
        public static void ForceTeamRespawn(bool isCI) => RespawnManager.Singleton.ForceSpawnTeam(isCI ? SpawnableTeamType.ChaosInsurgency : SpawnableTeamType.NineTailedFox);
        public static void CallCICar() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.ChaosInsurgency);
        public static void CallMTFHelicopter() => RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.Selection, SpawnableTeamType.NineTailedFox);
    }
}