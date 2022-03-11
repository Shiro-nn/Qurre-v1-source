using System;
using HarmonyLib;
using PlayerStatsSystem;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(HealthStat), nameof(HealthStat.ServerHeal))]
    internal static class Heal
    {
        private static bool Prefix(HealthStat __instance, float healAmount)
        {
            try
            {
                var ev = new HealEvent(API.Player.Get(__instance.Hub), healAmount);
                Qurre.Events.Invoke.Player.Heal(ev);
                if (ev.Allowed) __instance.CurValue = Mathf.Min(__instance.CurValue + Mathf.Abs(ev.Hp), ev.Player.MaxHp);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [Heal]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}