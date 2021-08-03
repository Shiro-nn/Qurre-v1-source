using System;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HealHPAmount))]
    internal static class Heal
    {
        private static bool Prefix(PlayerStats __instance, float hp)
        {
            try
            {
                var ev = new HealEvent(API.Player.Get(__instance.gameObject), hp);
                Qurre.Events.Invoke.Player.Heal(ev);
                if (ev.Allowed)
                {
                    float num = Mathf.Clamp(ev.Hp, 0f, __instance.maxHP - __instance.Health);
                    __instance.Health += num;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [UsingMedical]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}