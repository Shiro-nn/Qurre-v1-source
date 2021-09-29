using System;
using Assets._Scripts.Dissonance;
using HarmonyLib;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    //[HarmonyPatch(typeof(DissonanceUserSetup), nameof(DissonanceUserSetup.EnableSpeaking))]
    internal static class Speak
    {
        private static bool Prefix(DissonanceUserSetup __instance, TriggerType triggerType, Assets._Scripts.Dissonance.RoleType roleType = Assets._Scripts.Dissonance.RoleType.Null)
        {
            try
            {
                var ev = new SpeakEvent(__instance, __instance.IntercomAsHuman, __instance.RadioAsHuman,
                    __instance.MimicAs939, __instance.SCPChat, __instance.SpectatorChat, triggerType, roleType);
                Qurre.Events.Invoke.Player.Speak(ev);
                __instance.SCPChat = ev.ScpChat;
                __instance.SpectatorChat = ev.RipChat;
                __instance.IntercomAsHuman = ev.Intercom;
                __instance.MimicAs939 = ev.MimicAs939;
                __instance.RadioAsHuman = ev.Radio;
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