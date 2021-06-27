using System;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.SendRpc))]
    internal static class UsedMedical
    {
        private static void Prefix(ConsumableAndWearableItems __instance, ConsumableAndWearableItems.HealAnimation healAnimation, int mid)
        {
            try
            {
                if (healAnimation == (ConsumableAndWearableItems.HealAnimation)ConsumableAndWearableItems.HealAnimation.DequipMedicalItem)
                {
                    var ev = new MedicalUsedEvent(API.Player.Get(__instance.gameObject), __instance.usableItems[mid].inventoryID);
                    Qurre.Events.Invoke.Player.MedicalUsed(ev);
                }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [UsedMedical]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}