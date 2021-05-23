using System;
using Assets._Scripts.Dissonance;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.CallCmdAltIsActive))]
    internal static class Speak
    {
        private static bool Prefix(DissonanceUserSetup __instance, bool value)
        {
            try
            {
                var ev = new SpeakEvent(__instance, __instance.IntercomAsHuman, __instance.RadioAsHuman, __instance.MimicAs939, __instance.SCPChat, __instance.SpectatorChat, value);
                Qurre.Events.Player.speak(ev);
                __instance.SCPChat = ev.ScpChat;
                __instance.SpectatorChat = ev.RipChat;
                __instance.IntercomAsHuman = ev.Intercom;
                if (ev.MimicAs939) __instance.MimicAs939 = value;
                else __instance.MimicAs939 = false;
                if (ev.Radio) __instance.RadioAsHuman = value;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Speak]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}