#pragma warning disable SA1313
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.Alpha
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdSwitchAWButton))]
    internal static class EnablePanel
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute() ||
                    (__instance.InteractCuff().CufferId > 0 && !DisarmedInteract()))
                    return false;
                GameObject gameObject = GameObject.Find("OutsitePanelScript");
                if (!__instance.ChckDis(gameObject.transform.position))
                    return false;
                Item itemById = __instance.InteractInv().GetItemByID(__instance.InteractInv().curItem);
                if (!__instance.ServerRolesInteract().BypassMode && itemById == null)
                    return false;
                var list = new List<string>();
                list.Add("CONT_LVL_3");
                var ev = new EnablePanelEvent(Player.Get(__instance.gameObject), list);
                Qurre.Events.Alpha.enablepanel(ev);
                if (ev.IsAllowed && itemById.permissions.Intersect(ev.Permissions).Any())
                {
                    gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
                    __instance.OnInteract();
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Alpha.EnablePanel:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}