using HarmonyLib;

using Mirror;

using System;

using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;

namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(PlayableScps.Scp096), "PreWindup")]
    internal static class PreWindup
    {
        private static bool Prefix(PlayableScps.Scp096 __instance, float delay = 0.0f)
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
            catch (Exception ex)
            {
                Log.Error(string.Format("Error in patching Scp096 [PreWindup]:\n{0}\n{1}", ex, ex.StackTrace));
                return true;
            }
        }
    }
}
