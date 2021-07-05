using System;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.Disconnect), new[] { typeof(GameObject), typeof(string) })]
    internal static class Kick
    {
        private static bool Prefix(GameObject player, ref string message)
        {
            try
            {
                if (player == null) return false;
                var ev = new KickedEvent(API.Player.Get(player), message);
                Qurre.Events.Invoke.Player.Kicked(ev);
                message = ev.Reason;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Kick]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}