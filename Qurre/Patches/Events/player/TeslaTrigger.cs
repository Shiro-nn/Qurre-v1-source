﻿using System;
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
                if (__instance == null) return false;
                if (!NetworkServer.active) return false;
                foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs())
                {
                    if (allHub.Value.characterClassManager.CurClass == RoleType.Spectator)
                        continue;
                    foreach (TeslaGate teslaGate in __instance.TeslaGates)
                    {
                        if (teslaGate != null)
                        {
                            if (!teslaGate.PlayerInRange(allHub.Value) || teslaGate.InProgress) continue;
                            if (Player.Get(allHub.Key).Invisible) continue;
                            var ev = new TeslaTriggerEvent(Player.Get(allHub.Key), teslaGate.GetTesla(), teslaGate.PlayerInHurtRange(allHub.Key));
                            Qurre.Events.Invoke.Player.TeslaTrigger(ev);
                            if (ev.Triggerable && ev.Tesla.Enable
                                && !ev.Tesla.ImmunityRoles.Contains(ev.Player.Role)
                                && !ev.Tesla.ImmunityPlayers.Contains(ev.Player)) teslaGate.ServerSideCode();
                        }
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [TeslaTrigger]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}