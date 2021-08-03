﻿using System;
using HarmonyLib;
using Qurre.API.Events;
using Qurre.API;
using Mirror;
using System.Linq;
using System.Collections.Generic;

namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), new[] { typeof(NetworkConnection) })]
    internal static class Leave
    {
        private static void Prefix(NetworkConnection conn)
        {
            try
            {
                if (conn.identity == null || conn.identity.gameObject == null) return;
                Player player = Player.Get(conn.identity.gameObject);
                if (player == null || player.IsHost) return;
                var ev = new LeaveEvent(player);
                ServerConsole.AddLog($"Player {player.Nickname} ({player.UserId}) ({player?.Id}) disconnected", ConsoleColor.DarkMagenta);
                Qurre.Events.Invoke.Player.Leave(ev);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Leaving]:\n{e}\n{e.StackTrace}");
            }
        }
    }
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Leaved
    {
        private static void Prefix(ReferenceHub __instance)
        {
            try
            {
                Player player = Player.Get(__instance);
                if (player == null || player.IsHost) return;
                if (Player.Dictionary.ContainsKey(player.GameObject)) Player.Dictionary.Remove(player.GameObject);
                if (Player.IdPlayers.ContainsKey(player.Id)) Player.IdPlayers.Remove(player.Id);
                if (player.UserId != null) if (Player.UserIDPlayers.ContainsKey(player.UserId)) Player.UserIDPlayers.Remove(player.UserId);
                var fixList = new Dictionary<string, Player>();
                foreach (var pl in Player.ArgsPlayers.Where(x => x.Value == player)) fixList.Add(pl.Key, pl.Value);
                foreach (var pl in fixList) Player.ArgsPlayers.Remove(pl.Key);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Leaved]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}