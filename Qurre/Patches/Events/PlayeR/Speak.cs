﻿using System;
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
                var ev = new SpeakEvent(__instance, __instance.IntercomAsHuman, __instance.RadioAsHuman, __instance.MimicAs939, __instance.SCPChat, __instance.SpectatorChat);
                Qurre.Events.Player.speak(ev);
                __instance.SCPChat = ev.IsScpChat;
                __instance.SpectatorChat = ev.IsRipChat;
                __instance.IntercomAsHuman = ev.IsIntercom;
                if (ev.IsMimicAs939) __instance.MimicAs939 = value;
                else __instance.MimicAs939 = false;
                if (ev.IsRadio) __instance.RadioAsHuman = value;
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.Speak:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}