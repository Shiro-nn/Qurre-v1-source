#pragma warning disable SA1313
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using Respawning;
using Respawning.NamingRules;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class TeamRespawn
    {
        private static bool Prefix(RespawnManager __instance)
        {
            try
            {
                if (!RespawnWaveGenerator.SpawnableTeams.TryGetValue(__instance.NextKnownTeam, out SpawnableTeam spawnableTeam) ||
                    __instance.NextKnownTeam == SpawnableTeamType.None)
                    ServerConsole.AddLog($"umm, team {__instance.NextKnownTeam} is undefined", ConsoleColor.Red);
                else
                {
                    List<Player> list = Player.List.Where(p => p.Role == RoleType.Spectator && !p.Overwatch).ToList();
                    if (list.Count > 0)
                    {
                        RespawnTickets singleton = RespawnTickets.Singleton;
                        int tick = singleton.GetAvailableTickets(__instance.NextKnownTeam);
                        if (tick == 0)
                        {
                            tick = RespawnTickets.DefaultTeamAmount;
                            RespawnTickets.Singleton.GrantTickets(RespawnTickets.DefaultTeam, RespawnTickets.DefaultTeamAmount, true);
                        }
                        int avsp = Mathf.Min(tick, spawnableTeam.MaxWaveSize);
                        if (__instance.RespawnManager_prioritySpawn())
                            list = list.OrderBy(item => item.ClassManager.DeathTime).ToList();
                        else
                            list.ShuffleList();
                        List<Player> twolist = new List<Player>();
                        var ev = new TeamRespawnEvent(list, avsp, __instance.NextKnownTeam);
                        Qurre.Events.Round.teamrespawn(ev);
                        while (list.Count > avsp) list.RemoveAt(list.Count - 1);
                        list.ShuffleList();
                        foreach (Player targ in list)
                        {
                            try
                            {
                                RoleType classid = spawnableTeam.ClassQueue[Mathf.Min(twolist.Count, spawnableTeam.ClassQueue.Length - 1)];
                                targ.ClassManager.SetPlayersClass(classid, targ.GameObject);
                                twolist.Add(targ);
                            }
                            catch { }
                        }
                        if (twolist.Count > 0 && ev.Allowed)
                        {
                            RespawnTickets.Singleton.GrantTickets(__instance.NextKnownTeam, -twolist.Count * spawnableTeam.TicketRespawnCost);
                            if (UnitNamingRules.TryGetNamingRule(__instance.NextKnownTeam, out UnitNamingRule rule))
                            {
                                rule.GenerateNew(__instance.NextKnownTeam, out string regular);
                                foreach (Player pl in twolist)
                                {
                                    pl.ClassManager.NetworkCurSpawnableTeamType = (byte)__instance.NextKnownTeam;
                                    pl.ClassManager.NetworkCurUnitName = regular;
                                }
                                rule.PlayEntranceAnnouncement(regular);
                            }
                            RespawnEffectsController.ExecuteAllEffects(RespawnEffectsController.EffectType.UponRespawn, __instance.NextKnownTeam);
                        }
                        __instance.NextKnownTeam = SpawnableTeamType.None;
                    }
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Round [TeamRespawn]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}