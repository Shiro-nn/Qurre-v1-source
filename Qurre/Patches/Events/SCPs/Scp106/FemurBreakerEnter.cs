using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.AllowContain))]
    internal static class FemurBreakerEnter
    {
        private static bool Prefix(CharacterClassManager __instance)
        {
            try
            {
                if (!NetworkServer.active || !NonFacilityCompatibility.currentSceneSettings.enableStandardGamplayItems)
                    return false;
                foreach (GameObject player in PlayerManager.players)
                {
                    if (Vector3.Distance(player.transform.position, __instance._lureSpj.transform.position) <
                        1.97000002861023)
                    {
                        CharacterClassManager CCM = player.GetComponent<CharacterClassManager>();
                        PlayerStats Stats = player.GetComponent<PlayerStats>();
                        if (CCM.Classes.SafeGet(CCM.CurClass).team != Team.SCP &&
                            CCM.CurClass != RoleType.Spectator && !CCM.GodMode)
                        {
                            var ev = new FemurBreakerEnterEvent(API.Player.Get(Stats.gameObject));
                            Qurre.Events.Invoke.Scp106.FemurBreakerEnter(ev);
                            if (ev.Allowed)
                            {
                                Stats.HurtPlayer(new PlayerStats.HitInfo(10000f, "WORLD", DamageTypes.Lure, 0, true), player);
                                __instance._lureSpj.SetState(false, true);
                            }
                        }
                    }
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP106 [FemurBreakerEnter]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}