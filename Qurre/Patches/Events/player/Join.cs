using System;
using Qurre.API;
using HarmonyLib;
using MEC;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkIsVerified), MethodType.Setter)]
    internal static class Join
    {
        internal static void Prefix(CharacterClassManager __instance, bool value)
        {
            try
            {
                if (!value || string.IsNullOrEmpty(__instance?.UserId)) return;
                Player player = new Player(ReferenceHub.GetHub(__instance.gameObject));
                try
                {
                    if (!Player.Dictionary.ContainsKey(__instance.gameObject)) Player.Dictionary.Add(__instance.gameObject, player);
                    else Player.Dictionary[__instance.gameObject] = player;
                }
                catch { }
                ServerConsole.AddLog($"Player {player?.Nickname} ({player?.UserId}) ({player?.Id}) connected. iP: {player?.Ip}", ConsoleColor.Magenta);
                Timing.CallDelayed(0.25f, () =>
                {
                    if (player != null && player.Muted)
                        player.ClassManager.SetDirtyBit(1UL);
                });
                var ev = new JoinEvent(player);
                Qurre.Events.Invoke.Player.Join(ev);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Join]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}