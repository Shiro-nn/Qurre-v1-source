using HarmonyLib;
using Scp914;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Scp914Machine), "Start")]
    internal static class Scp914Controller
    {
        private static bool loaded = false;
        private static void Prefix(Scp914Machine __instance)
        {
            try
            {
                if (loaded) return;
                foreach (var recipe in __instance.recipes) API.Controllers.Scp914.RecipesList.Add(new API.Controllers.Scp914.Recipe(recipe));
                loaded = true;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [Scp914Controller]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}