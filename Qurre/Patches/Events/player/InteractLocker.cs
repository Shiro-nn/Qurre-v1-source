using System;
using System.Linq;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Events;
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
                if (colliderId >= __instance.Chambers.Length || !__instance.Chambers[colliderId].CanInteract) return false;
                bool allow = true;
                if (!__instance.CheckPerms(__instance.Chambers[colliderId].RequiredPermissions, ply) && !ply.serverRoles.BypassMode) allow = false;
                var chamber = __instance.Chambers[colliderId];
                var ev = new InteractLockerEvent(Player.Get(ply), locker, locker.Chambers.Where(x => x.LockerChamber == chamber).FirstOrDefault(), allow);
                Qurre.Events.Invoke.Player.InteractLocker(ev);
                if (!ev.Allowed) __instance.RpcPlayDenied(colliderId);
                else
                {
                    chamber.SetDoor(!chamber.IsOpen, __instance._grantedBeep);
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