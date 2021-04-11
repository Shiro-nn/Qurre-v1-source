using GameCore;
using HarmonyLib;
using MapGeneration;
using Mirror;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(SeedSynchronizer), "Start")]
    internal static class SetSeed
    {
        private static bool Prefix(SeedSynchronizer __instance)
        {
            try
            {
                if (!NetworkServer.active) return false;
                int num = ConfigFile.ServerConfig.GetInt("map_seed", -1);
                if (num < 1)
                {
                    num = UnityEngine.Random.Range(1, int.MaxValue);
                    SeedSynchronizer.DebugInfo("Server has successfully generated a random seed: " + num.ToString(), MessageImportance.Normal, false);
                }
                else SeedSynchronizer.DebugInfo("Server has successfully loaded a seed from config: " + num.ToString(), MessageImportance.Normal, false);
                __instance.Network_syncSeed = Mathf.Clamp(num, 1, int.MaxValue);
                var ev = new SetSeedEvent(__instance.Network_syncSeed);
                Qurre.Events.Map.setSeed(ev);
                __instance.Network_syncSeed = ev.Seed;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [SetSeed]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}