using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerChangeLock))]
    internal static class DoorLock
    {
        private static bool Prefix(DoorVariant __instance, DoorLockReason reason, bool newState)
        {
            try
            {
                try
                {
                    if (__instance == null || __instance.gameObject == null) return true;
                }
                catch
                {
                    return true;
                }
                var ev = new DoorLockEvent(Extensions.GetDoor(__instance), reason, newState);
                Qurre.Events.Invoke.Map.DoorLock(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [DoorLock]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(DoorEventOpenerExtension), nameof(DoorEventOpenerExtension.Trigger))]
    internal static class DoorOpen
    {
        private static bool Prefix(DoorEventOpenerExtension __instance, DoorEventOpenerExtension.OpenerEventType eventType)
        {
            try
            {
                if(__instance == null || __instance.TargetDoor == null || __instance.TargetDoor.gameObject == null) return true;
                var ev = new DoorOpenEvent(Extensions.GetDoor(__instance.TargetDoor), eventType);
                Qurre.Events.Invoke.Map.DoorOpen(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [DoorOpen]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}