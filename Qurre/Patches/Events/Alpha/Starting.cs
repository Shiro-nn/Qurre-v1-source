using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.Alpha
{
    using Qurre.API;
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdDetonateWarhead))]
    internal static class Starting
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!__instance.CanInteract) return false;
                if (!__instance._playerInteractRateLimit.CanExecute()) return false;
                GameObject gameObject = GameObject.Find("OutsitePanelScript");
                if (!__instance.ChckDis(gameObject.transform.position) || !AlphaWarheadOutsitePanel.nukeside.enabled || !gameObject.GetComponent<AlphaWarheadOutsitePanel>().keycardEntered) return false;
                var ev = new AlphaStartEvent(Player.Get(__instance.gameObject) ?? API.Server.Host);
                Qurre.Events.Invoke.Alpha.Starting(ev);
                if (!ev.Allowed) return false;
                AlphaWarheadController.Host.StartDetonation();
                ReferenceHub component = __instance.GetComponent<ReferenceHub>();
                ServerLogs.AddLog(ServerLogs.Modules.Warhead, component.LoggedNameFromRefHub() + " started the Alpha Warhead detonation.", ServerLogs.ServerLogType.GameEvent, false);
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