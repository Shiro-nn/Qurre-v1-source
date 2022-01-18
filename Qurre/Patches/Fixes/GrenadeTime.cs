/*using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using NorthwoodLib.Pools;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Items;
using UnityEngine;
using static HarmonyLib.AccessTools;
namespace Qurre.Patches.Fixes
{
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerThrow), typeof(float), typeof(float), typeof(Vector3))]
    internal static class GrenadeTime
    {
        public static Item GetItem(ItemBase itemBase) => Item.Get(itemBase);
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = -1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Callvirt) + offset;
            LocalBuilder timeGrenade = generator.DeclareLocal(typeof(TimeGrenade));
            LocalBuilder explosive = generator.DeclareLocal(typeof(ExplosiveGrenade));
            LocalBuilder flash = generator.DeclareLocal(typeof(FlashGrenade));
            LocalBuilder item = generator.DeclareLocal(typeof(Item));
            Label notExplosiveLabel = generator.DefineLabel();
            Label skipLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Isinst, typeof(TimeGrenade)),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(GrenadeTime), nameof(GrenadeTime.GetItem))),
                new CodeInstruction(OpCodes.Stloc, item.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, item.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                new CodeInstruction(OpCodes.Ldloc, item.LocalIndex),
                new CodeInstruction(OpCodes.Isinst, typeof(ExplosiveGrenade)),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, notExplosiveLabel),

                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.FuseTime))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.GTB))),
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Isinst, typeof(ExplosionGrenade)),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ExplosionGrenade, ExplosiveGrenade>), nameof(Dictionary<ExplosiveGrenade, ExplosionGrenade>.Add))),

                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                new CodeInstruction(OpCodes.Ret),

                new CodeInstruction(OpCodes.Ldloc, item.LocalIndex).WithLabels(notExplosiveLabel),
                new CodeInstruction(OpCodes.Isinst, typeof(FlashGrenade)),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.FuseTime))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(TimeGrenade), nameof(TimeGrenade._fuseTime))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.GTB))),
                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Isinst, typeof(FlashbangGrenade)),
                new CodeInstruction(OpCodes.Ldloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<FlashbangGrenade, FlashGrenade>), nameof(Dictionary<FlashbangGrenade, FlashGrenade>.Add))),

                new CodeInstruction(OpCodes.Ldloc, timeGrenade.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(TimeGrenade), nameof(TimeGrenade.ServerActivate))),
                new CodeInstruction(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }


    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PlayExplosionEffects))]
    internal static class GrenadeFieldsFrag
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;
            Label skipLabel = generator.DefineLabel();
            LocalBuilder explosive = generator.DeclareLocal(typeof(ExplosiveGrenade));

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.GTB))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloca_S, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ExplosionGrenade, ExplosiveGrenade>), nameof(Dictionary<ExplosionGrenade, ExplosiveGrenade>.TryGetValue))),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.BurnDuration))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._burnedDuration))),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.DeafenDuration))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._deafenedDuration))),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.ConcussDuration))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._concussedDuration))),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.ScpMultiplier))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._scpDamageMultiplier))),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.MaxRadius))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._maxRadius))),

                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }


    [HarmonyPatch(typeof(FlashbangGrenade), nameof(FlashbangGrenade.PlayExplosionEffects))]
    internal static class GrenadeFieldsFlash
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;
            Label skipLabel = generator.DefineLabel();
            LocalBuilder flash = generator.DeclareLocal(typeof(FlashGrenade));

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.GTB))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloca_S, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<FlashbangGrenade, FlashGrenade>), nameof(Dictionary<FlashbangGrenade, FlashGrenade>.TryGetValue))),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.BlindCurve))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade._blindingOverDistance))),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.DeafenCurve))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade._deafenDurationOverDistance))),

                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, flash.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(FlashGrenade), nameof(FlashGrenade.SurfaceDistanceIntensifier))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(FlashbangGrenade), nameof(FlashbangGrenade._surfaceZoneDistanceIntensifier))),

                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}*/