using System;
using System.Collections.Generic;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TeslaTrigger
    {
        private static bool Prefix(TeslaGateController __instance)
        {
            try
            {
                if (__instance is null) return false;
                if (!NetworkServer.active)
                {
                    foreach (TeslaGate teslaGate2 in __instance.TeslaGates)
                        teslaGate2.ClientSideCode();
                    return false;
                }
                var allHubs = ReferenceHub.GetAllHubs();
                foreach (TeslaGate teslaGate in __instance.TeslaGates)
                {
                    if (!teslaGate.isActiveAndEnabled) continue;

                    if (teslaGate.InactiveTime > 0f)
                    {
                        teslaGate.NetworkInactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
                        continue;
                    }

                    var tesla = teslaGate.GetTesla();
                    if (!tesla.Enable) continue;

                    bool idling = false;
                    bool activated = false;

                    foreach (KeyValuePair<GameObject, ReferenceHub> item in allHubs)
                    {
                        if (item.Value.isDedicatedServer) continue;
                        if (item.Value.characterClassManager.CurClass == RoleType.Spectator) continue;
                        bool inidl = teslaGate.PlayerInIdleRange(item.Value);
                        if (!inidl) continue;
                        var pl = Player.Get(item.Value);
                        if (pl.Invisible) continue;
                        bool inrng = teslaGate.PlayerInRange(item.Value);
                        var ev = new TeslaTriggerEvent(pl, tesla, inidl, inrng);
                        Qurre.Events.Invoke.Player.TeslaTrigger(ev);
                        if (!ev.Allowed) continue;
                        if (!idling) idling = true;
                        if (!activated && inrng && !teslaGate.InProgress)
                            activated = true;
                    }

                    if (activated)
                        teslaGate.ServerSideCode();
                    if (idling != teslaGate.isIdling)
                        teslaGate.ServerSideIdle(activated);
                }

                return false;
            }
            catch (NullReferenceException e)
            {
                Debug.Log(e);
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [TeslaTrigger]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}