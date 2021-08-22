using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using QurreModLoader;
using UnityEngine;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdDetonateWarhead))]
    internal static class Starting
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!__instance.CanInteract || !__instance._playerInteractRateLimit.CanExecute(true))
                    return false;
                GameObject gameObject = GameObject.Find("OutsitePanelScript");
                if (!__instance.ChckDis(gameObject.transform.position) || !AlphaWarheadOutsitePanel.nukeside.enabled || !gameObject.GetComponent<AlphaWarheadOutsitePanel>().keycardEntered)
                    return false;
                AlphaWarheadController.Host.doorsOpen = false;
                ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);
                if ((umm.AWC_resumeScenario() == -1 && AlphaWarheadController.Host.scenarios_start[umm.AWC_startScenario()].SumTime() == AlphaWarheadController.Host.timeToDetonation) ||
                    (umm.AWC_resumeScenario() != -1 && AlphaWarheadController.Host.scenarios_resume[umm.AWC_resumeScenario()].SumTime() == AlphaWarheadController.Host.timeToDetonation))
                {
                    var ev = new AlphaStartEvent(Player.Get(__instance.gameObject) ?? API.Server.Host);
                    Qurre.Events.Invoke.Alpha.Starting(ev);
                    if (!ev.Allowed) return false;
                    AlphaWarheadController.Host.NetworkinProgress = true;
                }
                __instance.OnInteract();
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Alpha [Starting]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}