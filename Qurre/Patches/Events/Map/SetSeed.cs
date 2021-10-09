using HarmonyLib;
using MapGeneration;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(SeedSynchronizer), nameof(SeedSynchronizer.Start))]
    internal static class SetSeed
    {
        private static void Postfix(SeedSynchronizer __instance)
        {
            try
            {
                var ev = new SetSeedEvent(__instance.Network_syncSeed);
                Qurre.Events.Invoke.Map.SetSeed(ev);
                __instance.Network_syncSeed = ev.Seed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map [SetSeed]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}