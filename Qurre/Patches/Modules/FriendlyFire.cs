using Footprinting;
using HarmonyLib;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using PlayableScps.Interfaces;
using Qurre.API;
using System;
using UnityEngine;
using static HitboxIdentity;
namespace Qurre.Patches.Modules
{
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.CheckFriendlyFire), new[] { typeof(ReferenceHub), typeof(ReferenceHub), typeof(bool) })]
    internal static class FriendlyFire
    {
        internal static bool Prefix(ref bool __result, ReferenceHub attacker)
        {
            if (Player.Get(attacker) == null) return true;
            __result = Player.Get(attacker).FriendlyFire || Server.FriendlyFire;
            return !__result;
        }
    }
    [HarmonyPatch(typeof(HitboxIdentity), nameof(HitboxIdentity.Damage))]
    internal static class FriendlyFire_Fix_Damage
    {
        internal static bool Prefix(HitboxIdentity __instance, ref bool __result, float damage, IDamageDealer item, Footprint attackerFootprint)
        {
            try
            {
                if (attackerFootprint.NetId != __instance.NetworkId)
                {
                    Role role = __instance.TargetHub.characterClassManager.Classes.SafeGet(attackerFootprint.Role);
                    Role curRole = __instance.TargetHub.characterClassManager.CurRole;
                    if (!HitboxIdentity.CheckFriendlyFire(attackerFootprint.Hub, __instance.TargetHub, false))
                    {
                        __result = false;
                        return false;
                    }
                    if (Misc.GetFaction(role.team) == Misc.GetFaction(curRole.team)) damage *= PlayerStats.FriendlyFireFactor;
                }
                HitboxIdentity.DamagePercent damagePercent = item.UseHitboxMultipliers ? __instance._dmgMultiplier : HitboxIdentity.DamagePercent.Body;
                if (item.UseHitboxMultipliers) damage *= (float)damagePercent / 100f;
                int bulletPenetrationPercent = Mathf.RoundToInt(item.ArmorPenetration * 100f);
                if (__instance.TargetHub.inventory.TryGetBodyArmor(out BodyArmor bodyArmor))
                {
                    if (damagePercent == HitboxIdentity.DamagePercent.Headshot) damage = BodyArmorUtils.ProcessDamage(bodyArmor.HelmetEfficacy, damage, bulletPenetrationPercent);
                    else if (damagePercent == HitboxIdentity.DamagePercent.Body) damage = BodyArmorUtils.ProcessDamage(bodyArmor.VestEfficacy, damage, bulletPenetrationPercent);
                }
                else if (__instance.TargetHub.scpsController.CurrentScp is PlayableScps.Interfaces.IArmoredScp armoredScp)
                    damage = BodyArmorUtils.ProcessDamage(armoredScp.GetArmorEfficacy(), damage, bulletPenetrationPercent);
                attackerFootprint.Hub.playerStats.HurtPlayer(new PlayerStats.HitInfo(damage, attackerFootprint.LoggedHubName, item.DamageType, attackerFootprint.PlayerId, false),
                    __instance.TargetHub.gameObject, false, true);
                __result = true;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Modules [FriendlyFire Fix Damage]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}