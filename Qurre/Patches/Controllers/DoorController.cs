using HarmonyLib;
using MapGeneration;
using Qurre.API;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(DoorSpawnpoint), "Start")]
    public class DoorController
    {
        private static bool Prefix(DoorSpawnpoint __instance)
        {
            try
            {
                if (Extensions.DoorVariant == null)
                    Extensions.DoorVariant = UnityEngine.Object.Instantiate(__instance.TargetPrefab);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [DoorController]:\n{e}\n{e.StackTrace}");
            }
            return true;
        }
    }
}