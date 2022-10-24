using System;
using System.Linq;
using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
    internal static class InteractLocker
    {
        private static bool Prefix(Locker __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                var locker = __instance.GetLocker();
                if (locker is null) return true;
                if (colliderId >= __instance.Chambers.Length) return false;
                var chmb = __instance.Chambers[colliderId];
                if (!chmb.CanInteract) return false;
                bool allow = true;
                if (!__instance.CheckPerms(chmb.RequiredPermissions, ply) && !ply.serverRoles.BypassMode) allow = false;
                var chamber = __instance.Chambers[colliderId];
                locker.Chambers.TryFind(out var chmbr, x => x.LockerChamber == chamber);
                var ev = new InteractLockerEvent(Player.Get(ply), locker, chmbr, allow);
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