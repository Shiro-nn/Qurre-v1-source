using System;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), new Type[] { typeof(ReferenceHub), typeof(byte) })]
    internal static class InteractDoor
    {
        private static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                var ev = new InteractDoorEvent(Player.Get(ply), __instance.GetDoor(), false);
                var Bypass = false;
                var Interact = false;
                if (__instance.ActiveLocks != 0)
                {
                    DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)__instance.ActiveLocks);
                    if ((!mode.HasFlagFast(DoorLockMode.CanClose)
                            || !mode.HasFlagFast(DoorLockMode.CanOpen))
                        && (!mode.HasFlagFast(DoorLockMode.ScpOverride)
                            || ply.characterClassManager.CurRole.team != 0)
                        && (mode == DoorLockMode.FullLock
                            || (__instance.TargetState
                                && !mode.HasFlagFast(DoorLockMode.CanClose))
                            || (!__instance.TargetState
                                && !mode.HasFlagFast(DoorLockMode.CanOpen))))
                    {
                        ev.Allowed = false;
                        Bypass = true;
                    }
                }
                if (!Bypass && (Interact = __instance.AllowInteracting(ply, colliderId)))
                {
                    if (ply.characterClassManager.CurClass == RoleType.Scp079 || __instance.RequiredPermissions.CheckPermissions(ply.inventory.CurInstance, ply)) ev.Allowed = true;
                    else ev.Allowed = false;
                }
                Qurre.Events.Invoke.Player.InteractDoor(ev);
                if (ev.Allowed && Interact)
                {
                    __instance.NetworkTargetState = !__instance.TargetState;
                    __instance._triggerPlayer(ply);
                }
                else if (Bypass)
                {
                    __instance.LockBypassDenied(ply, colliderId);
                }
                else if (Interact)
                {
                    __instance.PermissionsDenied(ply, colliderId);
                    DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [InteractDoor]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}