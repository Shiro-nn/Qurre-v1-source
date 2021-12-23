using HarmonyLib;
using Qurre.API.Events;
using Qurre.API;
using System.Linq;
using PlayerStatsSystem;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.KillPlayer))]
    internal static class Dead
    {
        private static void Postfix(PlayerStats __instance, DamageHandlerBase handler)
        {
            try
            {
                var attacker = handler.GetAttacker();
                Player target = Player.Get(__instance.gameObject);
                if (attacker is null) attacker = target;
                if ((target != null && (target.Role != RoleType.Spectator || target.GodMode || target.IsHost)) || attacker == null) return;
                target.Zoomed = false;
                var ev = new DeadEvent(attacker, target, handler, handler.GetDamageType());
                Qurre.Events.Invoke.Player.Dead(ev);
                if (attacker != target)
                {
                    attacker._kills.Add(new API.Objects.KillElement(attacker, target, handler.GetDamageType(), System.DateTime.Now));
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