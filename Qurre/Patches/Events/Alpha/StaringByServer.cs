using HarmonyLib;
using Qurre.API.Events;
using Subtitles;
using System;
using Utils.Networking;

namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
    internal static class StaringByServer
    {
        private static bool Prefix(AlphaWarheadController __instance, bool automatic = false, bool instant = false)
        {
            try
            {
                __instance._isAutomatic = automatic;

                __instance.doorsOpen = false;
                ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);
                if ((AlphaWarheadController._resumeScenario == -1 && Math.Abs(__instance.scenarios_start[AlphaWarheadController._startScenario].SumTime() -
                    __instance.timeToDetonation) < 0.0001f) || (AlphaWarheadController._resumeScenario != -1 &&
                    Math.Abs(__instance.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() - __instance.timeToDetonation) < 0.0001f))
                {
                    var ev = new AlphaStartEvent(API.Server.Host);
                    Qurre.Events.Invoke.Alpha.Starting(ev);
                    if (!ev.Allowed) return false;
                    if (!instant)
                    {
                        bool flag = AlphaWarheadController._resumeScenario < 0;
                        NetworkUtils.SendToAuthenticated(new SubtitleMessage(new SubtitlePart[1]
                        {
                        new SubtitlePart(flag ? SubtitleType.AlphaWarheadEngage : SubtitleType.AlphaWarheadResumed, new string[1]
                        {
                            (flag ? __instance.scenarios_start[AlphaWarheadController._startScenario].tMinusTime :
                            __instance.scenarios_resume[AlphaWarheadController._resumeScenario].tMinusTime).ToString()
                        })
                        }));
                    }
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