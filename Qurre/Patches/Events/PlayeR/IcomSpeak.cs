#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(Intercom), nameof(Intercom.CallCmdSetTransmit))]
    internal static class IcomSpeak
    {
        private static bool Prefix(Intercom __instance, bool player)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true) || Intercom.AdminSpeaking)
                    return false;
                var ev = new IcomSpeakEvent(player ? ReferenceHub.GetHub(__instance.gameObject) : null);
                if (player)
                {
                    if (!__instance.ServerAllowToSpeak())
                        return false;
                    Qurre.Events.Player.icomSpeak(ev);
                    if (ev.IsAllowed)
                        Intercom.host.RequestTransmission(__instance.gameObject);
                }
                else
                {
                    if (!(Intercom.host.Networkspeaker == __instance.gameObject))
                        return false;
                    Qurre.Events.Player.icomSpeak(ev);
                    if (ev.IsAllowed)
                        Intercom.host.RequestTransmission(null);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.IcomSpeak:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}