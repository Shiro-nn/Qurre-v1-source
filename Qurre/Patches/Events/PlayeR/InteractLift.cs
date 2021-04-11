#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseElevator), typeof(GameObject))]
    internal static class InteractLift
    {
        private static bool Prefix(PlayerInteract __instance, GameObject elevator)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true) ||
                    (__instance.InteractCuff().CufferId > 0 && !DisarmedInteract()) || elevator == null)
                    return false;
                Lift lift = elevator.GetComponent<Lift>();
                if (lift == null) return false;
                foreach (Lift.Elevator lIft in lift.elevators)
                {
                    if (__instance.ChckDis(lIft.door.transform.position))
                    {
                        var ev = new InteractLiftEvent(API.Player.Get(__instance.gameObject), lIft, lift.GetLift());
                        Qurre.Events.Player.interactLift(ev);
                        if (!ev.Allowed) return false;
                        elevator.GetComponent<Lift>().UseLift();
                        __instance.OnInteract();
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