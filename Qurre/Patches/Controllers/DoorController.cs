using HarmonyLib;
using MapGeneration;
using Qurre.API;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(DoorSpawnpoint), nameof(DoorSpawnpoint.Start))]
    internal static class DoorController
    {
        private static void Postfix(DoorSpawnpoint __instance)
        {
            try
            {
                string name = __instance.TargetPrefab.name.ToUpper();
                if(name.Contains("HCZ BREAKABLEDOOR") && Extensions.DoorPrefabHCZ == null) Extensions.DoorPrefabHCZ = UnityEngine.Object.Instantiate(__instance.TargetPrefab);
                else if (name.Contains("LCZ BREAKABLEDOOR") && Extensions.DoorPrefabLCZ == null) Extensions.DoorPrefabLCZ = UnityEngine.Object.Instantiate(__instance.TargetPrefab);
                else if (name.Contains("EZ BREAKABLEDOOR") && Extensions.DoorPrefabEZ == null) Extensions.DoorPrefabEZ = UnityEngine.Object.Instantiate(__instance.TargetPrefab);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [DoorController]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}