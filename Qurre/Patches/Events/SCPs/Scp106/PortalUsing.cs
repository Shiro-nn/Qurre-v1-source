using HarmonyLib;
using MEC;
using UnityEngine;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.UserCode_CmdUsePortal))]
    internal static class PortalUsing
    {
        private static bool Prefix(Scp106PlayerScript __instance)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute(false))
                    return false;
                var ev = new PortalUsingEvent(API.Player.Get(__instance.gameObject), __instance.portalPosition);
                Qurre.Events.Invoke.Scp106.PortalUsing(ev);
                __instance.portalPosition = ev.PortalPosition;
                if (!ev.Allowed)
                    return false;
                if (__instance.iAm106 && __instance.portalPosition != Vector3.zero && !__instance.goingViaThePortal)
                    Timing.RunCoroutine(__instance._DoTeleportAnimation(), Segment.Update);
                return true;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP106 [PortalUsing]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}