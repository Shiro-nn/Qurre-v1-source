using HarmonyLib;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
    internal static class StaringByServer
    {
        private static bool Prefix(AlphaWarheadController __instance)
        {
            try
            {
                if (Recontainer079.isLocked) return false;
                __instance.doorsOpen = false;
                if ((AWC_resumeScenario() != -1 || __instance.scenarios_start[AWC_startScenario()].SumTime() != (double)__instance.timeToDetonation) && (AWC_resumeScenario() == -1 || __instance.scenarios_resume[AWC_resumeScenario()].SumTime() != (double)__instance.timeToDetonation))
                    return false;
                var ev = new AlphaStartEvent(API.Server.Host);
                Qurre.Events.Invoke.Alpha.Starting(ev);
                if (!ev.Allowed) return false;
                __instance.NetworkinProgress = true;
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Alpha [StaringByServer]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}