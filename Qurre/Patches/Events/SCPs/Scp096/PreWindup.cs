using HarmonyLib;
using Mirror;
using Qurre.API.Events;
using System;
using scp096 = PlayableScps.Scp096;
namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(scp096), nameof(scp096.PreWindup))]
    internal static class PreWindup
    {
        private static bool Prefix(scp096 __instance, float delay = 0.0f)
        {
            try
            {
                if (!NetworkServer.active || __instance is null)
                    return false;
                var ev = new PreWindupEvent(__instance, API.Player.Get(__instance.Hub.gameObject), delay);
                Qurre.Events.Invoke.Scp096.PreWindup(ev);
                if (!ev.Allowed)
                    return false;
                __instance._preWindupTime = ev.Delay;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [PreWindup]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}