﻿using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using NorthwoodLib.Pools;
using Qurre.API.Controllers.Items;
using static HarmonyLib.AccessTools;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(ExplosionGrenade), nameof(ExplosionGrenade.PlayExplosionEffects))]
    internal static class ExplosiveGrenadeFieldsFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;
            Label skipLabel = generator.DefineLabel();
            LocalBuilder explosive = generator.DeclareLocal(typeof(ExplosiveGrenade));

            newInstructions.InsertRange(index, new[]
            {
                // if (!ExplosiveGrenade.GrenadeToItem.TryGetValue(this, out ExplosiveGrenade explosive)
                //     goto SKIP_LABEL
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.GrenadeToItem))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloca_S, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<ExplosionGrenade, ExplosiveGrenade>), nameof(Dictionary<ExplosionGrenade, ExplosiveGrenade>.TryGetValue))),
                new CodeInstruction(OpCodes.Brfalse, skipLabel),

                // this._burnedDuration = explosive.BurnDuration;
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.BurnDuration))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._burnedDuration))),

                // this._deafenedDuration = explosive.DeafenDuration
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.DeafenDuration))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._deafenedDuration))),

                // this._concussedDuration = explosive.ConcussDuration
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.ConcussDuration))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._concussedDuration))),

                // this._scpDamageMultiplier = explosive.ScpMultiplier
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.ScpMultiplier))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._scpDamageMultiplier))),

                // this._maxRadius = explosive.MaxRadius
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldloc, explosive.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ExplosiveGrenade), nameof(ExplosiveGrenade.MaxRadius))),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(ExplosionGrenade), nameof(ExplosionGrenade._maxRadius))),

                // SKIP_LABEL
                new CodeInstruction(OpCodes.Nop).WithLabels(skipLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}