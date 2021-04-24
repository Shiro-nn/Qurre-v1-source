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
                var ev = new DoorLockEvent(Extensions.GetDoor(__instance), reason, newState);
                Qurre.Events.Map.doorLock(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [DoorLock]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}