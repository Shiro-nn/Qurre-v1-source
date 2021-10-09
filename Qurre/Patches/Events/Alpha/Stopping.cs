using System;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), new Type[] { typeof(GameObject) })]
    internal static class Stopping
    {
        private static bool Prefix(AlphaWarheadController __instance, GameObject disabler)
        {
            try
			{
				if (!__instance.inProgress || __instance.timeToDetonation <= 10f || __instance._isLocked) return false;
                var ev = new AlphaStopEvent(API.Player.Get(disabler) ?? API.Server.Host);
                Qurre.Events.Invoke.Alpha.Stopping(ev);
                return ev.Allowed && !API.Controllers.Alpha.Locked;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Alpha [Stopping]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}