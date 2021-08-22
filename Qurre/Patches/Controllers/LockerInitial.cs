using HarmonyLib;
using MapGeneration.Distributors;
using Qurre.API;
using System;
using System.Linq;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
    internal static class LockerInitial
    {
        internal static void Postfix(Locker __instance)
        {
            try
            {
                while (Map.Lockers.Select(x => x.GameObject).Contains(null))
                    Map.Lockers.Remove(Map.Lockers.FirstOrDefault(x => x.GameObject == null));
                Map.Lockers.Add(new API.Controllers.Locker(__instance));
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [Locker]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}