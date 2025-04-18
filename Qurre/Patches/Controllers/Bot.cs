﻿using HarmonyLib;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using Mirror;
using Qurre.API;
using System;
using System.Reflection;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
    internal static class Bot
    {
        private static MethodInfo _mirror;
        private static MethodInfo _position;
        private static MethodInfo _showhit;
        internal static void Initialize()
        {
            //Log.Info("Patching Bot Controller");
            {
                var original = AccessTools.Method(typeof(NetworkBehaviour), "SendTargetRPCInternal");
                var method = typeof(Bot).GetMethod(nameof(Mirror));
                _mirror = PluginManager._harmony.Patch(original, new HarmonyMethod(method));
            }
            {
                var original = AccessTools.Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition));
                var method = typeof(Bot).GetMethod(nameof(Position));
                _position = PluginManager._harmony.Patch(original, new HarmonyMethod(method));
            }
            {
                var original = AccessTools.Method(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator));
                var method = typeof(Bot).GetMethod(nameof(ShowHit));
                _showhit = PluginManager._harmony.Patch(original, new HarmonyMethod(method));
            }
        }
        internal static void UnInitialize()
        {
            var mirror_original = AccessTools.Method(typeof(NetworkBehaviour), "SendTargetRPCInternal");
            var position_original = AccessTools.Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition));
            var showhit_original = AccessTools.Method(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator));
            try { if (_mirror is not null) PluginManager._harmony.Unpatch(mirror_original, _mirror); } catch { }
            try { if (_position is not null) PluginManager._harmony.Unpatch(position_original, _position); } catch { }
            try { if (_showhit is not null) PluginManager._harmony.Unpatch(showhit_original, _showhit); } catch { }
        }
        public static bool Mirror(NetworkBehaviour __instance)
        {
            if (!Round.BotSpawned) return true;
            var player = Player.Get(__instance.gameObject);
            if (player is not null && player.Bot) return false;
            return true;
        }
        public static bool Position(PlayerMovementSync __instance, Vector3 pos, PlayerMovementSync.PlayerRotation? rot = null, bool forceGround = false)
        {
            if (!Round.BotSpawned) return true;
            bool error_send = true;
            try
            {
                try { _ = __instance.transform.localScale; } catch { error_send = false; }
                if (forceGround && Physics.Raycast(pos, Vector3.down, out var hitInfo, 100f, __instance.CollidableSurfaces))
                    pos = hitInfo.point + Vector3.up * 1.23f;
                if (rot.HasValue) __instance.ForceRotation(rot.Value);
                __instance.ForcePosition(pos);
                return false;
            }
            catch (Exception e)
            {
                if (error_send) Log.Error($"umm, error in patching Controllers [Bot Position]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        public static bool ShowHit(uint netId, float damage, Vector3 origin)
        {
            if (!Round.BotSpawned) return true;
            try
            {
                if (!ReferenceHub.TryGetHubNetID(netId, out ReferenceHub hub)) return false;
                var pl = Player.Get(hub);
                if (pl is null || pl.Bot) return false;
                foreach (ReferenceHub hub2 in hub.spectatorManager.ServerCurrentSpectatingPlayers)
                {
                    hub2.networkIdentity.connectionToClient.Send(new GunHitMessage(false, damage, origin));
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Modules [Bot ShowHit]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}