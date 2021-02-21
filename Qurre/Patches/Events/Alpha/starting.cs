#pragma warning disable SA1118
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdDetonateWarhead))]
    internal static class Starting
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!QurreModLoader.umm.RateLimit(__instance).CanExecute(true) || (QurreModLoader.umm.InteractCuff(__instance).CufferId > 0 && !QurreModLoader.umm.DisarmedInteract()))
                    return false;
                GameObject gameObject = GameObject.Find("OutsitePanelScript");
                if (!__instance.ChckDis(gameObject.transform.position) || !AlphaWarheadOutsitePanel.nukeside.enabled)
                    return false;
                if (!gameObject.GetComponent<AlphaWarheadOutsitePanel>().keycardEntered || Recontainer079.isLocked)
                    return false;
                AlphaWarheadController.Host.doorsOpen = false;
                ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);
                if ((QurreModLoader.umm.AWC_resumeScenario() == -1 && AlphaWarheadController.Host.scenarios_start[QurreModLoader.umm.AWC_startScenario()].SumTime() == AlphaWarheadController.Host.timeToDetonation) ||
                    (QurreModLoader.umm.AWC_resumeScenario() != -1 && AlphaWarheadController.Host.scenarios_resume[QurreModLoader.umm.AWC_resumeScenario()].SumTime() == AlphaWarheadController.Host.timeToDetonation))
                {
                    var ev = new AlphaStartEvent(Player.Get(__instance.gameObject) ?? API.Map.Host);
                    Qurre.Events.Alpha.starting(ev);
                    if (!ev.IsAllowed)
                        return false;
                    AlphaWarheadController.Host.NetworkinProgress = true;
                }
                __instance.OnInteract();
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Alpha.Starting:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}