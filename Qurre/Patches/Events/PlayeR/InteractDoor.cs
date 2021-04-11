#pragma warning disable
using System;
using System.Linq;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), new Type[] { typeof(ReferenceHub), typeof(byte) })]
    internal static class InteractDoor
    {
        private static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                var ev = new InteractDoorEvent(API.Player.Get(ply), __instance.GetDoor(), false);
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
                    if (ply.characterClassManager.CurClass == RoleType.Scp079 || __instance.RequiredPermissions.CheckPermissions(ply.inventory.curItem, ply)) ev.Allowed = true;
                    else ev.Allowed = false;
                }
                Qurre.Events.Player.interactdoor(ev);
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