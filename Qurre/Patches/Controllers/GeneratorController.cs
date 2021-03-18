using HarmonyLib;
using Qurre.API;
using Qurre.API.Controllers;
using System;
using System.Linq;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Generator079), "Awake", new Type[] { typeof(Generator079) })]
    internal static class GeneratorController
    {
        private static void Postfix(Generator079 __instance)
        {
            try
            {
                while (Map.Generators.Select(x => x.GameObject).Contains(null))
                    Map.Generators.Remove(Map.Generators.FirstOrDefault(x => x.GameObject == null));
                if (!__instance.name.Contains("(")) Map.Generators.Add(new Generator(__instance, true));
                else Map.Generators.Add(new Generator(__instance, false));
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [GeneratorController]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}