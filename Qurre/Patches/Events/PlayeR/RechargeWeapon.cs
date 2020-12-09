#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;

namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.CallCmdReload))]
    internal static class RechargeWeapon
    {
        private static bool Prefix(WeaponManager __instance, bool animationOnly)
        {
            try
            {
                if (!QurreModLoader.umm.RateLimit(__instance).CanExecute(false))
                    return false;
                int iI = QurreModLoader.umm.Weapon_hub(__instance).inventory.GetItemIndex();
                if (iI < 0 || iI >= QurreModLoader.umm.Weapon_hub(__instance).inventory.items.Count ||
                    (__instance.curWeapon < 0 || QurreModLoader.umm.Weapon_hub(__instance).inventory.curItem !=
                        __instance.weapons[__instance.curWeapon].inventoryID) ||
                    QurreModLoader.umm.Weapon_hub(__instance).inventory.items[iI].durability >=
                    (double)__instance.weapons[__instance.curWeapon].maxAmmo)
                    return false;
                var ev = new RechargeWeaponEvent(ReferenceHub.GetHub(__instance.gameObject), animationOnly);
                Qurre.Events.Player.rechargeWeapon(ev);
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.RechargeWeapon:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}