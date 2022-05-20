using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using GameCore;
using HarmonyLib;
using MEC;
using Qurre.API.Events;
using RoundRestarting;
using UnityEngine;
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
                if (code.Value.operand is not null && code.Value.operand is MethodBase methodBase &&
                    methodBase.Name == nameof(RoundSummary._ProcessServerSideCode))
                    codes[code.Index].operand = CustomProcess;
            }
            return codes.AsEnumerable();
        }
        private static IEnumerator<float> ProcessServerSide(RoundSummary instance)
        {
            float time = Time.unscaledTime;
            while (instance is not null)
            {
                yield return Timing.WaitForSeconds(2.5f);
                while (RoundSummary.RoundLock || !RoundSummary.RoundInProgress() || Time.unscaledTime - time < 15f ||
                    (instance._keepRoundOnOne && PlayerManager.players.Count < 2))
                    yield return Timing.WaitForSeconds(1);

                bool end = false;
                int cw = 0;
                int mw = 0;
                int sw = 0;
                int nd = 0;
                int ns = 0;
                int ci = 0;
                int mtf = 0;
                int scp = 0;
                int zombies = 0;
                foreach (var pl in API.Player.List)
                {
                    switch (pl.Team)
                    {
                        case Team.CDP:
                            nd++;
                            break;
                        case Team.RSC:
                            ns++;
                            break;
                        case Team.CHI:
                            ci++;
                            break;
                        case Team.MTF:
                            mtf++;
                            break;
                        case Team.SCP:
                            {
                                scp++;
                                if (pl.Role is RoleType.Scp0492) zombies++;
                                break;
                            }
                    }
                }
                int d = RoundSummary.EscapedClassD + nd;
                int s = RoundSummary.EscapedScientists + ns;
                bool MTFAlive = mtf > 0;
                bool CiAlive = ci > 0;
                bool ScpAlive = scp > 0;
                bool DClassAlive = nd > 0;
                bool ScientistsAlive = ns > 0;
                var cList = new RoundSummary.SumInfo_ClassList
                {
                    class_ds = d,
                    scientists = s,
                    chaos_insurgents = ci,
                    mtf_and_guards = mtf,
                    scps_except_zombies = scp - zombies,
                    zombies = zombies,
                    warhead_kills = AlphaWarheadController.Host.detonated ? AlphaWarheadController.Host.warheadKills : -1,
                    time = (int)Time.realtimeSinceStartup
                };
                if (ScpAlive && !MTFAlive && !DClassAlive && !ScientistsAlive)
                {
                    end = true;
                    sw++;
                }
                else if (!ScpAlive && (MTFAlive || ScientistsAlive) && !DClassAlive && !CiAlive)
                {
                    end = true;
                    mw++;
                }
                else if (!ScpAlive && !MTFAlive && !ScientistsAlive && (DClassAlive || CiAlive))
                {
                    end = true;
                    cw++;
                }
                else if (!ScpAlive && !MTFAlive && !ScientistsAlive && !DClassAlive && !CiAlive)
                {
                    end = true;
                }
                var leading = LeadingTeam.Draw;
                if (d > s) cw++;
                else if (d < s) mw++;
                else if (scp > d + s) sw++;
                if (ci > mtf) cw++;
                else if (ci < mtf) mw++;
                else if (scp > ci + mtf) sw++;
                if (cw > mw)
                {
                    if (cw > sw) leading = LeadingTeam.ChaosInsurgency;
                    else if (mw < sw) leading = LeadingTeam.Anomalies;
                    else leading = LeadingTeam.Draw;
                }
                else if (mw > cw)
                {
                    if (mw > sw) leading = LeadingTeam.FacilityForces;
                    else if (cw < sw) leading = LeadingTeam.Anomalies;
                    else leading = LeadingTeam.Draw;
                }
                else leading = LeadingTeam.Draw;
                var ev = new CheckEvent(leading, cList, end);
                Qurre.Events.Invoke.Round.Check(ev);
                cList = ev.ClassList;
                instance.RoundEnded = ev.RoundEnd || API.Round.ForceEnd;
                leading = ev.LeadingTeam;
                if (instance.RoundEnded)
                {
                    FriendlyFireConfig.PauseDetector = true;
                    string text = $"Round finished! Anomalies: {scp} | Chaos: {ci} | Facility Forces: {mtf} | D escaped: {d} | Scientists escaped: {s}";
                    Console.AddLog(text, Color.gray);
                    ServerLogs.AddLog(ServerLogs.Modules.Logger, text, ServerLogs.ServerLogType.GameEvent);
                    yield return Timing.WaitForSeconds(0.5f);
                    int to_restart = Mathf.Clamp(ConfigFile.ServerConfig.GetInt("auto_round_restart_time", 10), 5, 1000);
                    if (instance is not null)
                    {
                        var end_ev = new RoundEndEvent(leading, cList, to_restart);
                        Qurre.Events.Invoke.Round.End(end_ev);
                        cList = end_ev.ClassList;
                        leading = end_ev.LeadingTeam;
                        to_restart = Mathf.Clamp(end_ev.ToRestart, 5, 1000);
                        instance.RpcShowRoundSummary(instance.classlistStart, cList, leading, RoundSummary.EscapedClassD,
                            RoundSummary.EscapedScientists, RoundSummary.KilledBySCPs, to_restart);
                    }
                    yield return Timing.WaitForSeconds(to_restart - 1);
                    instance.RpcDimScreen();
                    Timing.CallDelayed(1f, () => RoundRestart.InitiateRoundRestart());
                    try
                    {
                        foreach (var pl in API.Player.List)
                        {
                            try
                            {
                                if (pl.Role != RoleType.Spectator)
                                {
                                    pl.ClearInventory();
                                    pl.Role = RoleType.Spectator;
                                }
                            }
                            catch { }
                        }
                    }
                    catch { }
                    try
                    {
                        var __pics = API.Map.Pickups.ToArray();
                        foreach (var p in __pics) try { p.Destroy(); } catch { }
                    }
                    catch { }
                    try
                    {
                        var __dolls = API.Map.Ragdolls.ToArray();
                        foreach (var doll in __dolls) try { doll.Destroy(); } catch { }
                    }
                    catch { }
                    yield break;
                }
            }
        }
    }
}