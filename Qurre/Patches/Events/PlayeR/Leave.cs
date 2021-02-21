#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
using Qurre.API;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Leave
    {
        private static void Prefix(ReferenceHub __instance)
        {
            try
            {
                Player player = Player.Get(__instance);
                if (player == null || player.IsHost)
                    return;
                var ev = new LeaveEvent(player);
                ServerConsole.AddLog($"Player {ev.Player.Nickname} ({ev.Player.UserId}) ({player?.Id}) disconnected", ConsoleColor.DarkMagenta);
                Qurre.Events.Player.leave(ev);

                if (Player.Dictionary.ContainsKey(player.GameObject)) Player.Dictionary.Remove(player.GameObject);
                if (Player.IdPlayers.ContainsKey(player.Id)) Player.IdPlayers.Remove(player.Id);
                if (player.UserId != null) if (Player.UserIDPlayers.ContainsKey(player.UserId)) Player.UserIDPlayers.Remove(player.UserId);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.Leave:\n{e}\n{e.StackTrace}");
            }
        }
    }
}