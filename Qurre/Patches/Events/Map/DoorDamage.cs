using HarmonyLib;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Qurre.API;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(BreakableDoor), nameof(BreakableDoor.ServerDamage))]
    internal static class DoorDamage
    {
        private static bool Prefix(BreakableDoor __instance, float hp, DoorDamageType type)
        {
            try
            {
                var ev = new DoorDamageEvent(Extensions.GetDoor(__instance), hp, type);
                Qurre.Events.Map.doorDamage(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [DoorDamage]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}