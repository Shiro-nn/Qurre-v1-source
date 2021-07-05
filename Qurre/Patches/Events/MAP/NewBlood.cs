﻿using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.Map
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcPlaceBlood))]
    internal static class NewBlood
    {
        private static bool Prefix(CharacterClassManager __instance, ref Vector3 pos, ref int type, ref float f)
        {
            try
            {
                var ev = new NewBloodEvent(API.Player.Get(__instance.gameObject), pos, type, f);
                Qurre.Events.Invoke.Map.NewBlood(ev);
                pos = ev.Position;
                type = ev.Type;
                f = ev.Multiplier;
                return ev.Allowed && Plugin.Config.GetBool("Qurre_spawn_blood", true);
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Map [NewBlood]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.RpcPlaceDecal))]
    internal static class NewBloodAndDecal
    {
        private static bool Prefix(WeaponManager __instance, bool isBlood, ref int type, ref Vector3 pos, ref Quaternion rot)
        {
            try
            {
                if (isBlood)
                {
                    int bt = QurreModLoader.umm.Weapon_hub(__instance).characterClassManager.Classes.SafeGet(QurreModLoader.umm.Weapon_hub(__instance).characterClassManager.CurClass).bloodType;
                    var ev = new NewBloodEvent(API.Player.Get(__instance.gameObject), pos, bt, 1);
                    pos = ev.Position;
                    QurreModLoader.umm.Weapon_hub(__instance).characterClassManager.Classes.SafeGet(QurreModLoader.umm.Weapon_hub(__instance).characterClassManager.CurClass).bloodType = ev.Type;
                    Qurre.Events.Invoke.Map.NewBlood(ev);
                    return ev.Allowed && Plugin.Config.GetBool("Qurre_spawn_blood", true);
                }
                else
                {
                    var ev = new NewDecalEvent(API.Player.Get(__instance.gameObject), pos, rot, type);
                    Qurre.Events.Invoke.Map.NewDecal(ev);
                    pos = ev.Position;
                    rot = ev.Rotation;
                    type = ev.Type;
                    return ev.Allowed;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Map [NewBloodAndDecal]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}