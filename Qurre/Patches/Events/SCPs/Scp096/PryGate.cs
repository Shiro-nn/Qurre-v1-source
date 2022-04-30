using Qurre.API;
using Qurre.API.Events;
using HarmonyLib;
using System;
using UnityEngine;
using scp096 = PlayableScps.Scp096;
namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(scp096), nameof(scp096.PryGate))]
    internal static class PryGate
    {
        private static bool Prefix(scp096 __instance, Interactables.Interobjects.PryableDoor gate)
        {
            try
            {
                if (gate is null || __instance is null || !Mirror.NetworkServer.active) return true;
                if (!__instance.Charging || !__instance.Enraged) return false;
                if (gate.TargetState || gate.GetExactState() > 0f) return false;
                if (!gate.TryPryGate()) return false;
                var door = gate.GetDoor();
                var pl = Player.Get(__instance.Hub.gameObject);
                var evStart = new StartPryGateEvent(__instance, pl, door);
                Qurre.Events.Invoke.Scp096.StartPryGate(evStart);
                if (!evStart.Allowed) return false;
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
                    __instance.Hub.playerMovementSync.OverridePosition(transform.position, new PlayerMovementSync.PlayerRotation(null, transform.rotation.eulerAngles.y), true);
                }
                MEC.Timing.RunCoroutine(__instance.MoveThroughGate(gate));
                MEC.Timing.RunCoroutine(__instance.ResetGateAnim());
                MEC.Timing.CallDelayed(2f, () =>
                {
                    var evEnd = new EndPryGateEvent(__instance, pl, door);
                    Qurre.Events.Invoke.Scp096.EndPryGate(evEnd);
                });
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [PryGate]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}