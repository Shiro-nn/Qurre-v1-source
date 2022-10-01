using HarmonyLib;
using Mirror;
using PlayableScps.ScriptableObjects;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp173
{
    using Qurre.API;
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.ServerDoTantrum))]
    internal static class TantrumPlace
    {
        private static bool Prefix(PlayableScps.Scp173 __instance)
        {
            try
            {
                if (__instance._tantrumCooldownRemaining > 0f || __instance._isObserved) return false;
                var ev = new TantrumPlaceEvent(Player.Get(__instance.Hub));
                Qurre.Events.Invoke.Scp173.TantrumPlace(ev);
                if(!ev.Allowed) return false;
                GameObject gameObject = UnityEngine.Object.Instantiate(ScpScriptableObjects.Instance.Scp173Data.TantrumPrefab);
                gameObject.transform.position = __instance.Hub.playerMovementSync.RealModelPosition;
                NetworkServer.Spawn(gameObject);
                __instance._tantrumCooldownRemaining = ev.Cooldown;
                if (__instance._teslaGateController != null)
                    foreach (TeslaGate teslaGate in __instance._teslaGateController.TeslaGates)
                        if (teslaGate.PlayerInIdleRange(__instance.Hub))
                            teslaGate.TantrumsToBeDestroyed.Add(gameObject);
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> Scp173 [TantrumPlace]:\n{e}\n{e.StackTrace}");
            }
            return false;
        }
    }
}