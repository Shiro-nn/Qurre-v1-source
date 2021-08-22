using HarmonyLib;
using InventorySystem.Items.Firearms.Modules;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.PlaceBullethole))]
    internal static class PlaceBulletHole
    {
        private static bool Prefix(StandardHitregBase __instance, ref Ray ray, ref RaycastHit hit)
        {
            try
            {
                var ev = new PlaceBulletHoleEvent(API.Player.Get(__instance.Hub), ray, hit);
                Qurre.Events.Invoke.Map.PlaceBulletHole(ev);
                hit.point = ev.Position;
                hit.normal = ev.Rotation;
                ray = ev.Ray;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [PlaceBulletHole]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}