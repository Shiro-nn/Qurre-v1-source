using HarmonyLib;
using MEC;
using Qurre.API;
using Qurre.API.Controllers;
using Respawning;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    internal static class CassieController
    {
        private static bool Prefix(string words, bool makeHold, bool makeNoise)
        {
            if (Cassie.Lock) return false;
            try
            {
                foreach (Cassie _ in Map.Cassies.List())
                {
                    if (_.Message == words && _.Hold == makeHold && _.Noise == makeNoise)
                    {
                        Map.Cassies.Remove(_);
                        Timing.CallDelayed(NineTailedFoxAnnouncer.singleton.CalculateDuration(words), () => Cassie.End());
                        return true;
                    }
                }
                Cassie _cassie = new(words, makeHold, makeNoise);
                Map.Cassies.Add(_cassie, true);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [CassieController]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}