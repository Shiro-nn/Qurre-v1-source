using HarmonyLib;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
    internal static class StaringByServer
    {
        private static bool Prefix(AlphaWarheadController __instance)
        {
            try
            {
                __instance.doorsOpen = false;
                ServerLogs.AddLog(global::ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent, false);
                if ((AlphaWarheadController._resumeScenario == -1 &&
                    Math.Abs(__instance.scenarios_start[AlphaWarheadController._startScenario].SumTime() - __instance.timeToDetonation) < 0.0001f) ||
                    (AlphaWarheadController._resumeScenario != -1 &&
                    Math.Abs(__instance.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() - __instance.timeToDetonation) < 0.0001f))
                {
                    var ev = new AlphaStartEvent(API.Server.Host);
                    Qurre.Events.Invoke.Alpha.Starting(ev);
                    if (!ev.Allowed) return false;
                    __instance.NetworkinProgress = true;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Alpha [StaringByServer]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}