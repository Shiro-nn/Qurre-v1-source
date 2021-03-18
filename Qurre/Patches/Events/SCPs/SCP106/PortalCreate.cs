#pragma warning disable SA1118
using HarmonyLib;
using UnityEngine;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.SCPs.SCP106
{
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdMakePortal))]
    internal static class PortalCreate
    {
        private static bool Prefix(Scp106PlayerScript __instance)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true))
                    return false;
                bool rayCastHit = Physics.Raycast(new Ray(__instance.transform.position, -__instance.transform.up), out RaycastHit raycastHit, 10f, __instance.teleportPlacementMask);
                var ev = new PortalCreateEvent(API.Player.Get(__instance.gameObject), raycastHit.point - Vector3.up);
                Qurre.Events.SCPs.SCP106.portalcreate(ev);
                Debug.DrawRay(__instance.transform.position, -__instance.transform.up, Color.red, 10f);
                if (ev.Allowed && __instance.iAm106 && !__instance.goingViaThePortal && rayCastHit)
                    __instance.SetPortalPosition(ev.Position);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP106.PortalCreate:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}