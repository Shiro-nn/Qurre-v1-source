#pragma warning disable SA1313
using System;
using System.Collections.Generic;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
    internal static class TeslaTrigger
    {
        private static bool Prefix(TeslaGateController __instance)
        {
            try
            {
                foreach (KeyValuePair<GameObject, ReferenceHub> allHub in ReferenceHub.GetAllHubs())
                {
                    if (allHub.Value.characterClassManager.CurClass == RoleType.Spectator)
                        continue;
                    foreach (TeslaGate teslaGate in __instance.TeslaGates())
                    {
                        if (!teslaGate.PlayerInRange(allHub.Value) || teslaGate.InProgress)
                            continue;
                        var ev = new TeslaTriggerEvent(ReferenceHub.GetHub(allHub.Key), teslaGate.PlayerInHurtRange(allHub.Key));
                        Qurre.Events.Player.teslaTrigger(ev);
                        if (ev.IsTriggerable)
                            teslaGate.ServerSideCode();
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.TeslaTrigger:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}