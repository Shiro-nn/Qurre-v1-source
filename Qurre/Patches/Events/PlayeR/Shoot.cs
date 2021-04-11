using System;
using HarmonyLib;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.CallCmdShoot))]
    internal static class Shoot
    {
        private static bool Prefix(WeaponManager __instance, GameObject target, Vector3 targetPos)
        {
            try
            {
                if (!__instance.RateLimit().CanExecute(true)) return false;
                int itemIndex = __instance.Weapon_hub().inventory.GetItemIndex();
                if (itemIndex < 0 || itemIndex >= __instance.Weapon_hub().inventory.items.Count || __instance.curWeapon < 0 ||
                    ((__instance.Weapon_reloadCooldown() > 0.0 || __instance.Weapon_fireCooldown() > 0.0) &&
                     !__instance.isLocalPlayer) || __instance.Weapon_hub().inventory.curItem != __instance.weapons[__instance.curWeapon].inventoryID ||
                     __instance.Weapon_hub().inventory.items[itemIndex].durability <= 0.0)
                    return false;
                Player Target = null;
                if (target != null) Target = Player.Get(target);
                Player player = Player.Get(__instance.gameObject);
                var ev = new ShootingEvent(Player.Get(__instance.gameObject), target, targetPos, player.ItemInHand.GetWeaponType());
                Qurre.Events.Player.shooting(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Shoot]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}