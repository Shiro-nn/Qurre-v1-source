using HarmonyLib;
using PlayableScps;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.SCPs.SCP096
{
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.Windup))]
    internal static class WindUp
    {
        private static bool Prefix(Scp096 __instance, bool force = false)
        {
            try
            {
                var ev = new WindupEvent(__instance, API.Player.Get(__instance.Hub.gameObject), force);
                Qurre.Events.Invoke.Scp096.Windup(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [Enrage]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}