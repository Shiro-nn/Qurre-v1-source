#pragma warning disable SA1118
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using Qurre.API;
using UnityEngine;
using static Qurre.API.Events.Report;
namespace Qurre.Patches.Events.Server.Report
{
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.CallCmdReport), typeof(int), typeof(string), typeof(byte[]), typeof(bool))]
    internal static class Local
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var list = new List<CodeInstruction>(instructions);
            var lastNG = list.FindLastIndex(ci => ci.opcode == OpCodes.Ldarg_S && (byte)ci.operand == 4) + 2;
            if (lastNG < 1) return list;
            var rE = generator.DefineLabel();
            list[lastNG].labels.Add(rE);
            list.InsertRange(lastNG, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc_2),
                new CodeInstruction(OpCodes.Ldarga_S, 2),
                new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(typeof(Local), nameof(InvokeLocalReport), new[] { typeof(CheaterReport), typeof(GameObject), typeof(string).MakeByRefType() })),
                new CodeInstruction(OpCodes.Brtrue_S, rE),
                new CodeInstruction(OpCodes.Ret),
            });
            return list;
        }
        private static bool InvokeLocalReport(CheaterReport reporter, GameObject reportedTo, ref string reason)
        {
            try
            {
                var issuer = Player.Get(reporter.gameObject);
                var target = Player.Get(reportedTo);
                var ev = new LocalEvent(issuer, target, reason);
                Qurre.Events.Server.Report.local(ev);
                reason = ev.Reason;
                if (!ev.IsAllowed && reporter.gameObject != reportedTo.gameObject)
                    reporter.GetComponent<GameConsoleTransmission>().SendToClient(reporter.connectionToClient, "[Local Report] Successfully", "green");
                return ev.IsAllowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Server.Report.Local:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}