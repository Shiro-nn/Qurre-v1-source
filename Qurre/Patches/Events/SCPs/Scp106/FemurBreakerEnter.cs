using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
using PlayerStatsSystem;
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
                foreach (ReferenceHub value in ReferenceHub.GetAllHubs().Values)
                {
                    if (!value.isDedicatedServer && value.Ready && Vector3.Distance(value.transform.position, __instance._lureSpj.transform.position) < 1.97f)
                    {
                        CharacterClassManager characterClassManager = value.characterClassManager;
                        PlayerStats playerStats = value.playerStats;
                        if (characterClassManager.CurRole.team != 0 && characterClassManager.CurClass != RoleType.Spectator && !characterClassManager.GodMode)
                        {
                            var ev = new FemurBreakerEnterEvent(API.Player.Get(value));
                            Qurre.Events.Invoke.Scp106.FemurBreakerEnter(ev);
                            if (ev.Allowed)
                            {
                                playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.UsedAs106Bait));
                                __instance._lureSpj.SetState(p: false, b: true);
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