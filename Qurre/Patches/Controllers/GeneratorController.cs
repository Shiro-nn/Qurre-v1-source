using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using Qurre.API.Controllers;
using System;
using System.Linq;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.Start))]
    internal static class GeneratorController
    {
        private static void Postfix(Scp079Generator __instance)
        {
            try
            {
                while (Map.Generators.Select(x => x.GameObject).Contains(null))
                    Map.Generators.Remove(Map.Generators.FirstOrDefault(x => x.GameObject == null));
                Map.Generators.Add(new Generator(__instance));
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [GeneratorController]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}