using HarmonyLib;
using MapGeneration;
using Qurre.API;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(DoorSpawnpoint), "Start")]
    internal static class DoorController
    {
        private static bool Prefix(DoorSpawnpoint __instance)
        {
            try
            {
                switch (__instance.TargetPrefab.name.ToUpper())
                {
                    case "HCZ BREAKABLEDOOR":
                        if (Extensions.DoorPrefabHCZ == null)
                            Extensions.DoorPrefabHCZ = UnityEngine.Object.Instantiate(__instance.TargetPrefab);
                        break;
                    case "LCZ BREAKABLEDOOR":
                        if (Extensions.DoorPrefabLCZ == null)
                            Extensions.DoorPrefabLCZ = UnityEngine.Object.Instantiate(__instance.TargetPrefab);
                        break;
                    case "EZ BREAKABLEDOOR":
                        if (Extensions.DoorPrefabEZ == null)
                            Extensions.DoorPrefabEZ = UnityEngine.Object.Instantiate(__instance.TargetPrefab);
                        break;
                }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [DoorController]:\n{e}\n{e.StackTrace}");
            }
            return true;
        }
    }
}