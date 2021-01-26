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
                ReferenceHub player = ReferenceHub.GetHub(__instance.gameObject);
                if (player == null || player.IsHost())
                    return;
                var ev = new LeaveEvent(player);
                ServerConsole.AddLog($"Player {ev.Player.GetNickname()} ({ev.Player.GetUserId()}) ({player?.GetPlayerId()}) disconnected", ConsoleColor.DarkMagenta);
                Qurre.Events.Player.leave(ev);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.Leave:\n{e}\n{e.StackTrace}");
            }
        }
    }
}