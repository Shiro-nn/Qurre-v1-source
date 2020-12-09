#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseLocker))]
    internal static class InteractLocker
    {
        private static bool Prefix(PlayerInteract __instance, byte lockerId, byte chamberNumber)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true) ||
                    (__instance.InteractCuff().CufferId > 0 && !DisarmedInteract()))
                    return false;
                LockerManager singleton = LockerManager.singleton;
                if (lockerId >= singleton.lockers.Length)
                    return false;
                if (!__instance.ChckDis(singleton.lockers[lockerId].gameObject.position) ||
                    !singleton.lockers[lockerId].supportsStandarizedAnimation)
                    return false;
                if (chamberNumber >= singleton.lockers[lockerId].chambers.Length)
                    return false;
                if (singleton.lockers[lockerId].chambers[chamberNumber].doorAnimator == null)
                    return false;
                if (!singleton.lockers[lockerId].chambers[chamberNumber].CooldownAtZero())
                    return false;
                singleton.lockers[lockerId].chambers[chamberNumber].SetCooldown();
                string accessToken = singleton.lockers[lockerId].chambers[chamberNumber].accessToken;
                Item item = __instance.InteractInv().GetItemByID(__instance.InteractInv().curItem);
                var ev = new InteractLockerEvent(
                    ReferenceHub.GetHub(__instance.gameObject),
                    singleton.lockers[lockerId],
                    singleton.lockers[lockerId].chambers[chamberNumber],
                    lockerId,
                    chamberNumber,
                    string.IsNullOrEmpty(accessToken) || (item != null && item.permissions.Contains(accessToken)) || __instance.ServerRolesInteract().BypassMode);
                Qurre.Events.Player.interactLocker(ev);
                if (ev.IsAllowed)
                {
                    bool boolean = (singleton.openLockers[lockerId] & 1 << chamberNumber) != 1 << chamberNumber;
                    singleton.ModifyOpen(lockerId, chamberNumber, boolean);
                    singleton.RpcDoSound(lockerId, chamberNumber, boolean);
                    bool anyOpen = true;
                    for (int i = 0; i < singleton.lockers[lockerId].chambers.Length; i++)
                    {
                        if ((singleton.openLockers[lockerId] & 1 << i) == 1 << i)
                        {
                            anyOpen = false;
                            break;
                        }
                    }
                    singleton.lockers[lockerId].LockPickups(!boolean, chamberNumber, anyOpen);
                    if (!string.IsNullOrEmpty(accessToken))
                        singleton.RpcChangeMaterial(lockerId, chamberNumber, false);
                }
                else
                    singleton.RpcChangeMaterial(lockerId, chamberNumber, true);
                __instance.OnInteract();
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.InteractLocker:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}