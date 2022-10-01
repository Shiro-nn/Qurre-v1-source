using System;
using HarmonyLib;
using InventorySystem;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdProcessHotkey))]
    internal static class HotKeyPress
    {
        internal static bool Prefix(Inventory __instance, ActionName hotkeyButtonPressed)
        {
            try
            {
                var ev = new HotKeyPressEvent(Player.Get(__instance._hub), hotkeyButtonPressed.GetKey());
                Qurre.Events.Invoke.Player.HotKeyPress(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [HotKeyPress]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}