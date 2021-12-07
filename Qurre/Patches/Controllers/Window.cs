using HarmonyLib;
using Qurre.API;
using System;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.ServerDamageWindow))]
    internal static class Window
    {
        private static bool Prefix(BreakableWindow __instance)
        {
            try
            {
                var window = __instance.GetWindow();
                if (window == null) return true;
                return window.AllowBreak;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [Window]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}