using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUseElevator), typeof(GameObject))]
    internal static class InteractLift
    {
        private static bool Prefix(PlayerInteract __instance, GameObject elevator)
        {
            try
            {
                if (!__instance.CanInteract) return false;
                if (elevator == null) return false;
                Lift component = elevator.GetComponent<Lift>();
                if (component == null) return false;
                foreach (Lift.Elevator elevator2 in component.elevators)
                {
                    if (__instance.ChckDis(elevator2.door.transform.position))
                    {
                        var ev = new InteractLiftEvent(Player.Get(__instance.gameObject), elevator2, component.GetLift());
                        Qurre.Events.Invoke.Player.InteractLift(ev);
                        if (ev.Allowed)
                        {
                            elevator.GetComponent<Lift>().UseLift();
                            __instance.OnInteract();
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [InteractLift]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}