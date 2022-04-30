using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
using MapGeneration;
using Qurre.API;
using PlayerStatsSystem;
using PDTT = PocketDimensionTeleport.PDTeleportType;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class PocketTryEscape
    {
        private static bool Prefix(PocketDimensionTeleport __instance, Collider other)
        {
            try
            {
                if (!NetworkServer.active) return false;

                NetworkIdentity component = other.GetComponent<NetworkIdentity>();
                if (component == null) return false;
                if(!ReferenceHub.TryGetHubNetID(component.netId, out var hub)) return false;
                var pl = Player.Get(hub);
                if (pl == null) return false;
                if (hub.characterClassManager.GodMode || __instance._type == PDTT.Exit)
                {
                    __instance.SuccessEscape(hub);
                }
                else if ((__instance._type == PDTT.Killer || BlastDoor.OneDoor.isClosed))
                {
                    var ev = new PocketFailEscapeEvent(pl, __instance);
                    Qurre.Events.Invoke.Scp106.PocketFailEscape(ev);
                    if (!ev.Allowed) return false;
                    hub.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.PocketDecay));
                }
                ImageGenerator.pocketDimensionGenerator.GenerateRandom();
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP106 [PocketTryEscape]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}