using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using NorthwoodLib.Pools;
using Qurre.API.Events;
using Respawning;
using Respawning.NamingRules;
using UnityEngine;
namespace Qurre.Patches.Events.Round
{
    using Qurre.API;
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class TeamRespawn
    {
        private static bool Prefix(RespawnManager __instance)
        {
            try
            {
                if (!RespawnWaveGenerator.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out SpawnableTeamHandlerBase spawnableTeamHandlerBase) || __instance.NextKnownTeam == SpawnableTeamType.None)
                {
                    ServerConsole.AddLog("Fatal error. Team '" + __instance.NextKnownTeam + "' is undefined.", ConsoleColor.Red);
                    return false;
                }
                List<Player> list = Player.List.Where(p => p.Role == RoleType.Spectator && !p.Overwatch).ToList();
                if (__instance._prioritySpawn) list = (from item in list orderby item.ClassManager.DeathTime select item).ToList();
                else list.ShuffleList();
                int num = RespawnTickets.Singleton.GetAvailableTickets(__instance.NextKnownTeam);
                if (RespawnTickets.Singleton.IsFirstWave) RespawnTickets.Singleton.IsFirstWave = false;
                if (num == 0)
                {
                    num = 5;
                    RespawnTickets.Singleton.GrantTickets(SpawnableTeamType.ChaosInsurgency, 5, true);
                }
                int num2 = Mathf.Min(num, spawnableTeamHandlerBase.MaxWaveSize);
                var ev = new TeamRespawnEvent(list, num2, __instance.NextKnownTeam);
                Qurre.Events.Invoke.Round.TeamRespawn(ev);
                if (!ev.Allowed)
                {
                    __instance.NextKnownTeam = SpawnableTeamType.None;
                    return false;
                }
                list = ev.Players;
                num2 = ev.MaxRespAmount;
                while (list.Count > num2) list.RemoveAt(list.Count - 1);
                list.ShuffleList();
                List<Player> list2 = ListPool<Player>.Shared.Rent();
                Queue<RoleType> queue = new();
                spawnableTeamHandlerBase.GenerateQueue(queue, list.Count);
                foreach (Player pl in list)
                {
                    try
                    {
                        RoleType classid = queue.Dequeue();
                        pl.ClassManager.SetPlayersClass(classid, pl.GameObject, CharacterClassManager.SpawnReason.Respawn, false);
                        list2.Add(pl);
                        ServerLogs.AddLog(ServerLogs.Modules.ClassChange, $"Player {pl.ReferenceHub.LoggedNameFromRefHub()} respawned as {classid}.", ServerLogs.ServerLogType.GameEvent, false);
                    }
                    catch (Exception ex)
                    {
                        if (pl is not null && pl.ReferenceHub is not null)
                            ServerLogs.AddLog(ServerLogs.Modules.ClassChange, $"Player {pl.ReferenceHub.LoggedNameFromRefHub()} couldn't be spawned. Err msg: {ex.Message}",
                                ServerLogs.ServerLogType.GameEvent, false);
                        else ServerLogs.AddLog(ServerLogs.Modules.ClassChange, "Couldn't spawn a player - target's ReferenceHub is null.", ServerLogs.ServerLogType.GameEvent, false);
                    }
                }
                if (list2.Count > 0)
                {
                    ServerLogs.AddLog(ServerLogs.Modules.ClassChange, $"RespawnManager has successfully spawned {list2.Count} players as {__instance.NextKnownTeam}!",
                        ServerLogs.ServerLogType.GameEvent, false);
                    RespawnTickets.Singleton.GrantTickets(__instance.NextKnownTeam, -list2.Count * spawnableTeamHandlerBase.TicketRespawnCost, false);
                    if (UnitNamingRules.TryGetNamingRule(__instance.NextKnownTeam, out UnitNamingRule unitNamingRule))
                    {
                        unitNamingRule.GenerateNew(__instance.NextKnownTeam, out string text);
                        foreach (Player pl in list2)
                        {
                            pl.ClassManager.NetworkCurSpawnableTeamType = (byte)__instance.NextKnownTeam;
                            pl.ClassManager.NetworkCurUnitName = text;
                        }
                        unitNamingRule.PlayEntranceAnnouncement(text);
                    }
                    RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
                }
                ListPool<ReferenceHub>.Shared.Return(list2.Select(x => x.ReferenceHub).ToList());
                __instance.NextKnownTeam = SpawnableTeamType.None;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Round [TeamRespawn]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}