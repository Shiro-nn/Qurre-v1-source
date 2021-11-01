using HarmonyLib;
using InventorySystem.Items.Pickups;
using MapGeneration.Distributors;
using NorthwoodLib.Pools;
using System.Collections.Generic;
using System.Reflection.Emit;
using static HarmonyLib.AccessTools;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(LockerChamber), nameof(LockerChamber.SetDoor))]
    internal static class FixLockerDisconnect
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int labelOffset = 8;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Dup);
            LocalBuilder pickupBaseLocal = generator.DeclareLocal(typeof(ItemPickupBase));
            Label continueLabel = generator.DefineLabel();
            newInstructions.InsertRange(index + labelOffset, new[]
            {
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, pickupBaseLocal.LocalIndex),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Call, Method(typeof(UnityEngine.Object), nameof(UnityEngine.Object.Equals))),
                new CodeInstruction(OpCodes.Brtrue, continueLabel),
                new CodeInstruction(OpCodes.Ldloc, pickupBaseLocal.LocalIndex),
            });
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}