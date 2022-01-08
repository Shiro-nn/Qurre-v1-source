using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameCore;
using HarmonyLib;
using MEC;
using Qurre.API.Controllers.Items;
using Qurre.API.Events;
using RoundRestarting;
using UnityEngine;
using Rd = Qurre.API.Controllers.Ragdoll;
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
        private static IEnumerator<float> ProcessServerSide(RoundSummary instance)
        {
            float time = Time.unscaledTime;
            while (instance != null)
            {
                yield return Timing.WaitForSeconds(2.5f);
                while (RoundSummary.RoundLock || !RoundSummary.RoundInProgress() || Time.unscaledTime - time < 15f ||
                    (instance._keepRoundOnOne && PlayerManager.players.Count < 2))
                    yield return Timing.WaitForOneFrame;
                RoundSummary.SumInfo_ClassList newList = default;
                foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs())
                {
                    if (allHub.Value == null) continue;
                    CharacterClassManager characterClassManager = allHub.Value.characterClassManager;
                    if (characterClassManager.Classes.CheckBounds(characterClassManager.CurClass))
                    {
                        switch (characterClassManager.CurRole.team)
                        {
                            case Team.CDP:
                                newList.class_ds++;
                                break;
                            case Team.CHI:
                                newList.chaos_insurgents++;
                                break;
                            case Team.MTF:
                                newList.mtf_and_guards++;
                                break;
                            case Team.RSC:
                                newList.scientists++;
                                break;
                            case Team.SCP:
                                if (characterClassManager.CurClass == RoleType.Scp0492)
                                    newList.zombies++;
                                else newList.scps_except_zombies++;
                                break;
                        }
                    }
                }
                yield return Timing.WaitForOneFrame;
                newList.warhead_kills = AlphaWarheadController.Host.detonated ? AlphaWarheadController.Host.warheadKills : -1;
                yield return Timing.WaitForOneFrame;
                newList.time = (int)Time.realtimeSinceStartup;
                yield return Timing.WaitForOneFrame;
                RoundSummary.roundTime = newList.time - instance.classlistStart.time;
                int mtf_team = newList.mtf_and_guards + newList.scientists;
                int d_team = newList.chaos_insurgents + newList.class_ds;
                int scp_team = newList.scps_except_zombies + newList.zombies;
                int num4 = newList.class_ds + RoundSummary.EscapedClassD;
                int num5 = newList.scientists + RoundSummary.EscapedScientists;
                float num6 = (instance.classlistStart.class_ds != 0) ? (num4 / instance.classlistStart.class_ds) : 0;
                float num7 = (instance.classlistStart.scientists == 0) ? 1 : (num5 / instance.classlistStart.scientists);
                if (newList.class_ds == 0 && mtf_team == 0) instance.RoundEnded = true;
                else
                {
                    int num8 = 0;
                    if (mtf_team > 0) num8++;
                    if (d_team > 0) num8++;
                    if (scp_team > 0) num8++;
                    if (num8 <= 1) instance.RoundEnded = true;
                }
                bool flag = num5 > 0;
                bool flag2 = num4 > 0;
                bool num9 = mtf_team > 0;
                bool flag3 = scp_team > 0;
                LeadingTeam leadingTeam = LeadingTeam.Draw;
                if (num9)
                {
                    if (flag) leadingTeam = LeadingTeam.FacilityForces;
                }
                else if (flag2) leadingTeam = LeadingTeam.ChaosInsurgency;
                else if (flag3) leadingTeam = LeadingTeam.Anomalies;
                var ev = new CheckEvent(leadingTeam, newList, instance.RoundEnded);
                Qurre.Events.Invoke.Round.Check(ev);
                newList = ev.ClassList;
                instance.RoundEnded = ev.RoundEnd;
                leadingTeam = ev.LeadingTeam;
                if (API.Round.ForceEnd) instance.RoundEnded = API.Round.ForceEnd;
                if (instance.RoundEnded)
                {
                    FriendlyFireConfig.PauseDetector = true;
                    string text = $"Round finished! Anomalies: {scp_team} | Chaos: {d_team} | Facility Forces: {mtf_team} | D escaped percentage: {num6} | S escaped percentage: : {num7}";
                    GameCore.Console.AddLog(text, Color.gray);
                    ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent);
                    yield return Timing.WaitForSeconds(1.5f);
                    int num10 = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);
                    if (instance != null)
                    {
                        var end = new RoundEndEvent(leadingTeam, newList, num10);
                        Qurre.Events.Invoke.Round.End(end);
                        newList = end.ClassList;
                        leadingTeam = end.LeadingTeam;
                        num10 = end.ToRestart;
                        instance.RpcShowRoundSummary(instance.classlistStart, newList, leadingTeam, RoundSummary.EscapedClassD,
                            RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, num10);
                    }
                    yield return Timing.WaitForSeconds(num10 - 1);
                    instance.RpcDimScreen();
                    Timing.CallDelayed(1f, () => RoundRestart.InitiateRoundRestart());
                    try
                    {
                        var __list = API.Player.List.Where(x => x.Role != RoleType.Spectator);
                        foreach (var pl in __list)
                        {
                            try
                            {
                                pl.ClearInventory();
                                pl.Role = RoleType.Spectator;
                            }
                            catch { }
                        }
                    }
                    catch { }
                    try
                    {
                        var __pics = new List<Pickup>();
                        foreach (var item in API.Map.Pickups) try { __pics.Add(item); } catch { }
                        foreach (var p in __pics) try { p.Destroy(); } catch { }
                    }
                    catch { }
                    try
                    {
                        var __dolls = new List<Rd>();
                        foreach (var d in API.Map.Ragdolls) try { __dolls.Add(d); } catch { }
                        foreach (var d in __dolls) try { d.Destroy(); } catch { }
                    }
                    catch { }
                }
            }
        }
    }
}