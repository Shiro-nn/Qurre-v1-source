using System;
using InventorySystem.Items.Usables.Scp330;
using Qurre.API.Events;
using HarmonyLib;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerOnUsingCompleted))]
    internal static class EatingScp330
    {
        internal static bool Prefix(Scp330Bag __instance)
        {
            try
            {
                if (!__instance.IsCandySelected) return false;
                if (!Scp330Candies.CandiesById.TryGetValue(__instance.Candies[__instance.SelectedCandyId], out ICandy value)) return false;

                var ev = new EatingScp330Event(Player.Get(__instance.Owner), value.Kind);
                Qurre.Events.Invoke.Player.EatingScp330(ev);
                if (!ev.Allowed) return false;
                { if (value.Kind != ev.Candy && Scp330Candies.CandiesById.TryGetValue(ev.Candy, out ICandy nc)) value = nc; }

                __instance.IsUsing = false;
                value.ServerApplyEffects(__instance.Owner);
                __instance.Candies.RemoveAt(__instance.SelectedCandyId);
                __instance.OwnerInventory.ServerSelectItem(0);
                __instance.ServerRefreshBag();
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [EatingScp330]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}