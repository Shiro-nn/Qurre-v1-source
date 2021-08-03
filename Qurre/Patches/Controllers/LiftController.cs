using HarmonyLib;
using Qurre.API;
using System;
using System.Linq;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Lift), "Start")]
    internal static class LiftController
    {
        private static void Postfix(Lift __instance)
        {
            try
            {
                while (Map.Lifts.Select(x => x.GameObject).Contains(null))
                    Map.Lifts.Remove(Map.Lifts.FirstOrDefault(x => x.GameObject == null));
                Map.Lifts.Add(new API.Controllers.Lift(__instance));
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [LiftController]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}