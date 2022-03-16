using HarmonyLib;
using Qurre.API.Events;
using Qurre.API;
using System.Linq;
using PlayerStatsSystem;
using Qurre.API.Addons;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal static class Dead
    {
        private static bool Prefix(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                var attacker = handler.GetAttacker();
                Player target = Player.Get(__instance.gameObject);
                if (attacker is null) attacker = target;
                if ((target != null && (target.GodMode || target.IsHost)) || attacker == null) return true;
                var ev = new DiesEvent(attacker, target, handler, handler.GetDamageType());
                Qurre.Events.Invoke.Player.Dies(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [Dies]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
        private static void Postfix(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                var attacker = handler.GetAttacker();
                Player target = Player.Get(__instance.gameObject);
                if (attacker is null) attacker = target;
                if ((target != null && (target.Role != RoleType.Spectator || target.GodMode || target.IsHost)) || attacker == null) return;
                target.Zoomed = false;
                var type = handler.GetDamageType();
                var ev = new DeadEvent(attacker, target, handler, type);
                Qurre.Events.Invoke.Player.Dead(ev);
                if (attacker != target && attacker is not null && target is not null)
                {
                    attacker._kills.Add(new KillElement(attacker, target, type, System.DateTime.Now));
                    target.DeathsCount++;
                }
                if (target.Bot) API.Map.Bots.FirstOrDefault(x => x.Player == target).Destroy();
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [Dead]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}