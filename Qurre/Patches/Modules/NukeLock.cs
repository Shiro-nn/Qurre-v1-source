﻿using HarmonyLib;

namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(PlayerInteract), "UserCode_CmdUsePanel")]
    internal static class NukeLock
    {
        private static bool Prefix() => !API.Controllers.Alpha.InsidePanel.Locked;
    }
}