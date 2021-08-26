using System;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Events;
using KeyPerms = Interactables.Interobjects.DoorUtils.KeycardPermissions;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
    internal static class InteractLocker
    {
        private static bool Prefix(Locker __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                var locker = __instance.GetLocker();
                if (locker == null) return true;
                if (colliderId >= locker.Chambers.Length || !locker.Chambers[colliderId].CanInteract) return false;
                bool allow = true;
                if (!__instance.CheckPerms((KeyPerms)locker.Chambers[colliderId].Permissions, ply) && !ply.serverRoles.BypassMode) allow = false;
                var chamber = locker.Chambers[colliderId];
                var ev = new InteractLockerEvent(Player.Get(ply), locker, chamber, allow);
                Qurre.Events.Invoke.Player.InteractLocker(ev);
                if (!ev.Allowed) __instance.RpcPlayDenied(colliderId);
                else
                {
                    chamber.Opened = !chamber.Opened;
                    __instance.RefreshOpenedSyncvar();
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [InteractLocker]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}