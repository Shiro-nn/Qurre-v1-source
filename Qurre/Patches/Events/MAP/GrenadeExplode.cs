#pragma warning disable SA1313
using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using Grenades;
using HarmonyLib;
using UnityEngine;
using Qurre.API.Events;
using GameCore;
using static QurreModLoader.umm;
using Qurre.API;
namespace Qurre.Patches.Events.MAP.grenade
{
    [HarmonyPatch(typeof(FlashGrenade), nameof(FlashGrenade.ServersideExplosion))]
    internal static class Explode_flash
    {
        private static bool Prefix(FlashGrenade __instance)
        {
            try
            {
                Dictionary<Player, float> players = new Dictionary<Player, float>();
                foreach (GameObject gameObject in PlayerManager.players)
                {
                    Vector3 position = __instance.transform.position;
                    ReferenceHub hub = ReferenceHub.GetHub(gameObject);
                    Flashed effect = hub.playerEffectsController.GetEffect<Flashed>();
                    Deafened effect2 = hub.playerEffectsController.GetEffect<Deafened>();
                    if (effect == null || __instance.throwerGameObject == null ||
                        (!__instance.Network_friendlyFlash && !effect.Flashable(
                            ReferenceHub.GetHub(__instance.throwerGameObject), position, __instance.FlashGrenade_ignoredLayers())))
                        continue;
                    float num = __instance.powerOverDistance.Evaluate(
                                    Vector3.Distance(gameObject.transform.position, position) / ((position.y > 900f)
                                        ? __instance.distanceMultiplierSurface
                                        : __instance.distanceMultiplierFacility)) *
                                __instance.powerOverDot.Evaluate(Vector3.Dot(hub.PlayerCameraReference.forward, (hub.PlayerCameraReference.position - position).normalized));
                    byte b = (byte)Mathf.Clamp(Mathf.RoundToInt(num * 10f * __instance.maximumDuration), 1, 255);
                    if (b >= effect.Intensity && num > 0f)
                        players.Add(Player.Get(gameObject), num);
                }
                GrenadeExplodeEvent ev = new GrenadeExplodeEvent(Player.Get(__instance.throwerGameObject), players, false, __instance.gameObject);
                Qurre.Events.Map.grenadeexplode(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map -> Grenade [Explode Flash]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(FragGrenade), nameof(FragGrenade.ServersideExplosion))]
    internal static class Explode_frag
    {
        private static bool Prefix(FragGrenade __instance)
        {
            try
            {
                Vector3 vec = __instance.transform.position;
                Dictionary<Player, float> players = new Dictionary<Player, float>();
                foreach (GameObject gameObject in PlayerManager.players)
                {
                    if (ServerConsole.FriendlyFire || !(gameObject != __instance.throwerGameObject) || gameObject.GetComponent<WeaponManager>().GetShootPermission(__instance.throwerTeam, false))
                    {
                        PlayerStats PlSt = gameObject.GetComponent<PlayerStats>();
                        if (PlSt == null || !PlSt.ccm.InWorld) continue;
                        float fl = __instance.damageOverDistance.Evaluate(Vector3.Distance(vec, PlSt.transform.position)) * (PlSt.ccm.IsHuman() ?
                            ConfigFile.ServerConfig.GetFloat("human_grenade_multiplier", 0.7f) : ConfigFile.ServerConfig.GetFloat("scp_grenade_multiplier", 1f));
                        if (fl > __instance.absoluteDamageFalloff) players.Add(Player.Get(gameObject), fl);
                    }
                }
                var ev = new GrenadeExplodeEvent(Player.Get(__instance.throwerGameObject), players, true, __instance.gameObject);
                Qurre.Events.Map.grenadeexplode(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Map -> Grenade [Explode Frag]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}