using System;
using HarmonyLib;
using InventorySystem.Searching;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ContinuePickupServer))]
    internal static class PickupItem
    {
        private static bool Prefix(SearchCoordinator __instance)
        {
            try
            {
                var item = __instance.Completor.TargetPickup;
                if (item == null) return true;
                if (!__instance.Completor.ValidateUpdate())
                {
                    __instance.SessionPipe.Invalidate();
                    return false;
                }
                if (NetworkTime.time < __instance.SessionPipe.Session.FinishTime) return false;
                var ev = new PickupItemEvent(Player.Get(__instance.Hub), item.GetItem());
                Qurre.Events.Invoke.Player.PickupItem(ev);
                if (ev.Allowed) __instance.Completor.Complete();
                else __instance.SessionPipe.Invalidate();
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [PickupItem]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}