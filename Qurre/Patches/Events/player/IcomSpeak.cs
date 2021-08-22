using System;
using HarmonyLib;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.UserCode_CmdSetTransmit))]
    internal static class IcomSpeak
    {
        private static bool Prefix(Intercom __instance, bool player)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true) || Intercom.AdminSpeaking) return false;
                var ev = new IcomSpeakEvent(player ? API.Player.Get(__instance.gameObject) : null);
                if (player)
                {
                    if (!__instance.ServerAllowToSpeak()) return false;
                    Qurre.Events.Invoke.Player.IcomSpeak(ev);
                    if (ev.Allowed) Intercom.host.RequestTransmission(__instance.gameObject);
                }
                else
                {
                    if (!(Intercom.host.Networkspeaker == __instance.gameObject)) return false;
                    Qurre.Events.Invoke.Player.IcomSpeak(ev);
                    if (ev.Allowed) Intercom.host.RequestTransmission(null);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [IcomSpeak]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}