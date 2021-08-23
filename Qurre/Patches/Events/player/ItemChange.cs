﻿using HarmonyLib;
using InventorySystem;
using InventorySystem.Items;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.CurInstance), MethodType.Setter)]
    internal static class ItemChange
    {
        internal static bool Prefix(Inventory __instance, ItemBase value)
        {
            try
            {
                if (__instance.CurInstance == value) return false;
                var ev = new ItemChangeEvent(Player.Get(__instance._hub), Item.Get(__instance.CurInstance), Item.Get(value));
                Qurre.Events.Invoke.Player.ItemChange(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [ItemChange]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}