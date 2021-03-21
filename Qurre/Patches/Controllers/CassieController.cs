using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using Respawning;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(RespawnEffectsController), nameof(RespawnEffectsController.PlayCassieAnnouncement))]
    public class CassieController
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
                        Cassie.End();
                        return true;
                    }
                }
                Cassie _cassie = new Cassie(words, makeHold, makeNoise);
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