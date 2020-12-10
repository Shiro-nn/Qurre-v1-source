#pragma warning disable SA1313
using System;
using Qurre.API;
using HarmonyLib;
using MEC;
using Qurre.API.Events;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkIsVerified), MethodType.Setter)]
    internal static class Join
    {
        private static void Prefix(CharacterClassManager __instance, bool value)
        {
            try
            {
                if (!value || string.IsNullOrEmpty(__instance?.UserId))
                    return;
                ReferenceHub player = ReferenceHub.GetHub(__instance.gameObject);
                ServerConsole.AddLog($"Player {player?.GetNickname()} ({player?.GetUserId()}) ({player?.GetPlayerId()}) connected", ConsoleColor.Magenta);
                Timing.CallDelayed(0.25f, () =>
                {
                    if (player != null && player.IsMuted())
                        player.characterClassManager.SetDirtyBit(1UL);
                });
                var ev = new JoinEvent(ReferenceHub.GetHub(__instance.gameObject));
                Qurre.Events.Player.join(ev);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.Join:\n{e}\n{e.StackTrace}");
            }
        }
    }
}