using Qurre.API;
using Qurre.API.Events;
using HarmonyLib;
using System;
using UnityEngine;

namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(PlayableScps.Scp096), "PryGate")]
    internal static class PryGate
    {
        private static bool Prefix(PlayableScps.Scp096 __instance, Interactables.Interobjects.PryableDoor gate)
        {
            try
            {
                if (gate is null || __instance is null || !Mirror.NetworkServer.active)
                    return true;

                StartPryGateEvent ev = new StartPryGateEvent(__instance, Player.Get(__instance.Hub.gameObject), Extensions.GetDoor(gate));

                if (!__instance.Charging || !__instance.Enraged)
                    return false;

                if (gate.TargetState || gate.GetExactState() > 0f)
                    return false;

                if (!gate.TryPryGate())
                    return false;

                Qurre.Events.Invoke.Scp096.StartPryGate(ev);

                if (!ev.Allowed)
                    return false;

                __instance.Hub.fpc.NetworkforceStopInputs = true;
                __instance.PlayerState = PlayableScps.Scp096PlayerState.PryGate;
                float num = float.PositiveInfinity;
                Transform transform = null;

                foreach (Transform transform2 in gate.PryPositions)
                {
                    float num2 = Vector3.Distance(__instance.Hub.playerMovementSync.RealModelPosition, transform2.position);
                    if (num2 < num)
                    {
                        num = num2;
                        transform = transform2;
                    }
                }

                if (transform != null)
                {
                    float rot = transform.rotation.eulerAngles.y - __instance.Hub.PlayerCameraReference.rotation.eulerAngles.y;
                    __instance.Hub.playerMovementSync.OverridePosition(transform.position, rot, true);
                }

                MEC.Timing.RunCoroutine(__instance.MoveThroughGate(gate));
                MEC.Timing.RunCoroutine(__instance.ResetGateAnim());

                MEC.Timing.CallDelayed(2f, delegate ()
                {
                    EndPryGateEvent Ev = new EndPryGateEvent(__instance, Player.Get(__instance.Hub.gameObject), Extensions.GetDoor(gate));
                    Qurre.Events.Invoke.Scp096.EndPryGate(Ev);
                });

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Error in patching Scp096 [PryGate]:\n{0}\n{1}", ex, ex.StackTrace));
                return true;
            }
        }
    }
}
