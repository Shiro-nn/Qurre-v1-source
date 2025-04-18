﻿using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.Alpha
{
    using Qurre.API;
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdSwitchAWButton))]
    internal static class EnablePanel
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!__instance.CanInteract) return false;
                GameObject gameObject = GameObject.Find("OutsitePanelScript");
                if(gameObject == null) return false;
                if (!__instance.ChckDis(gameObject.transform.position)) return false;
                InventorySystem.Items.Keycards.KeycardItem keycardItem = __instance._inv.CurInstance as InventorySystem.Items.Keycards.KeycardItem;
                if (__instance._sr.BypassMode || (keycardItem is not null && keycardItem.Permissions.HasFlag(Interactables.Interobjects.DoorUtils.KeycardPermissions.AlphaWarhead)))
                {
                    var ev = new EnableAlphaPanelEvent(Player.Get(__instance.gameObject), keycardItem.Permissions);
                    Qurre.Events.Invoke.Alpha.EnablePanel(ev);
                    if (!ev.Allowed) return false;
                    gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
                    __instance.OnInteract();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Alpha [EnablePanel]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}