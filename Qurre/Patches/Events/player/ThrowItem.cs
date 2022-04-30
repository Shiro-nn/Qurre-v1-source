using System;
using HarmonyLib;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(ThrowableItem), nameof(ThrowableItem.ServerProcessInitiation))]
    internal static class ThrowItemPatch
    {
        private static bool Prefix(ThrowableItem __instance)
        {
            try
            {
                if (!__instance.AllowHolster) return false;
                var ev = new ThrowItemEvent(Player.Get(__instance.Owner), Item.Get(__instance.ItemSerial));
                Qurre.Events.Invoke.Player.ThrowItem(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [ThrowItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}