using HarmonyLib;
using Qurre.API.Events;
using System;
using scp096 = PlayableScps.Scp096;
namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(scp096), nameof(scp096.Windup))]
    internal static class WindUp
    {
        private static bool Prefix(scp096 __instance, bool force = false)
        {
            try
            {
                var ev = new WindupEvent(__instance, API.Player.Get(__instance.Hub.gameObject), force);
                Qurre.Events.Invoke.Scp096.Windup(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [WindUp]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}