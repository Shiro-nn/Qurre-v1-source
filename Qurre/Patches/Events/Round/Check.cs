#pragma warning disable SA1313
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameCore;
using HarmonyLib;
using MEC;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.Start))]
    internal static class Check
    {
        private static readonly MethodInfo CustomProcess = SymbolExtensions.GetMethodInfo(() => ProcessServerSide(null));
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            var codes = new List<CodeInstruction>(instr);
            foreach (var code in codes.Select((x, i) => new { Value = x, Index = i }))
            {
                if (code.Value.opcode != OpCodes.Call) continue;
                if (code.Value.operand != null && code.Value.operand is MethodBase methodBase &&
                    methodBase.Name == nameof(RoundSummary._ProcessServerSideCode))
                    codes[code.Index].operand = CustomProcess;
            }
            return codes.AsEnumerable();
        }
        public static IEnumerator<float> ProcessServerSide(RoundSummary instance)
        {
            while (instance != null)
            {
                while (RoundSummary.RoundLock || !RoundSummary.RoundInProgress() || (instance.RoundSummary_keepRoundOnOne() && PlayerManager.players.Count < 2)
                    || RoundStart.RoundLenght.TotalSeconds <= 3)
                    yield return Timing.WaitForOneFrame;
                RoundSummary.SumInfo_ClassList list = default;
                foreach (GameObject player in PlayerManager.players)
                    if (player != null)
                    {
                        CharacterClassManager component = player.GetComponent<CharacterClassManager>();
                        if (component.Classes.CheckBounds(component.CurClass))
                            switch (component.Classes.SafeGet(component.CurClass).team)
                            {
                                case Team.SCP:
                                    if (component.CurClass == RoleType.Scp0492)
                                    {
                                        ++list.zombies;
                                        continue;
                                    }
                                    ++list.scps_except_zombies;
                                    continue;
                                case Team.MTF:
                                    ++list.mtf_and_guards;
                                    continue;
                                case Team.CHI:
                                    ++list.chaos_insurgents;
                                    continue;
                                case Team.RSC:
                                    ++list.scientists;
                                    continue;
                                case Team.CDP:
                                    ++list.class_ds;
                                    continue;
                                default:
                                    continue;
                            }
                    }
                list.warhead_kills = AlphaWarheadController.Host.detonated ? AlphaWarheadController.Host.warheadKills : -1;
                yield return Timing.WaitForOneFrame;
                list.time = (int)Time.realtimeSinceStartup;
                yield return Timing.WaitForOneFrame;
                RoundSummary.roundTime = list.time - instance.classlistStart.time;
                int mtf_team = list.mtf_and_guards + list.scientists;
                int d_team = list.chaos_insurgents + list.class_ds;
                int scp_team = list.scps_except_zombies + list.zombies;
                if (list.class_ds == 0 && mtf_team == 0) instance.RoundSummary_roundEnded(true);
                else
                {
                    int count = 0;
                    if (mtf_team > 0) ++count;
                    if (d_team > 0) ++count;
                    if (scp_team > 0) ++count;
                    if (count <= 1) instance.RoundSummary_roundEnded(true);
                }
                instance.RoundSummary_roundEnded(API.Round.ForceEnd);
                var ev = new CheckEvent((RoundSummary.LeadingTeam)RoundSummary.LeadingTeam.Draw, list, instance.RoundSummary_roundEnded());
                if (mtf_team > 0)
                    if (RoundSummary.escaped_ds == 0 && RoundSummary.escaped_scientists != 0)
                        ev.LeadingTeam = (RoundSummary.LeadingTeam)RoundSummary.LeadingTeam.FacilityForces;
                    else
                        ev.LeadingTeam = RoundSummary.escaped_ds != 0 ? (RoundSummary.LeadingTeam)RoundSummary.LeadingTeam.ChaosInsurgency : (RoundSummary.LeadingTeam)RoundSummary.LeadingTeam.Anomalies;
                Qurre.Events.Round.check(ev);
                instance.RoundSummary_roundEnded(ev.RoundEnd);
                if (instance.RoundSummary_roundEnded())
                {
                    byte i1;
                    for (i1 = 0; i1 < 75; ++i1)
                        yield return 0.0f;
                    int timeToRoundRestart = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);
                    if (instance != null)
                    {
                        list.scps_except_zombies -= list.zombies;
                        var end = new RoundEndEvent(ev.LeadingTeam, list, timeToRoundRestart);
                        Qurre.Events.Round.end(end);
                        instance.RpcShowRoundSummary(instance.classlistStart, end.ClassList, (LeadingTeam)end.LeadingTeam, RoundSummary.escaped_ds, RoundSummary.escaped_scientists, RoundSummary.kills_by_scp, end.ToRestart);

                        var dpercentage = (float)instance.classlistStart.class_ds == 0 ? 0 : RoundSummary.escaped_ds + list.class_ds / instance.classlistStart.class_ds;
                        var spercentage = (float)instance.classlistStart.scientists == 0 ? 0 : RoundSummary.escaped_scientists + list.scientists / instance.classlistStart.scientists;
                        var text = $"Round finished! Anomalies: {scp_team} | Chaos: {d_team}" +
                            $" | Facility Forces: {mtf_team} | D escaped percentage: {dpercentage} | S escaped percentage : {spercentage}";
                        GameCore.Console.AddLog(text, Color.yellow, false);
                    }
                    for (int i2 = 0; i2 < 50 * (timeToRoundRestart - 1); ++i2)
                        yield return 0.0f;
                    instance.RpcDimScreen();
                    for (i1 = 0; i1 < 50; ++i1)
                        yield return 0.0f;
                    PlayerManager.localPlayer.GetComponent<PlayerStats>().Roundrestart();
                    yield break;
                }
            }
        }
    }
}