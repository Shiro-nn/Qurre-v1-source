using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch]
    internal static class Waiting
    {
        private static MethodBase TargetMethod() =>
            AccessTools.Method(AccessTools.FirstInner(typeof(CharacterClassManager), (Type x) => x.Name.Contains("<Init>")), "MoveNext");
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool need = true;
            foreach (var ins in instructions)
            {
                if (need && ins.opcode == OpCodes.Ldstr && ins.operand as string == "Waiting for players...")
                {
                    need = false;
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Qurre.Events.Invoke.Round), nameof(Qurre.Events.Invoke.Round.Waiting)));
                }
                yield return ins;
            }
        }
    }
}