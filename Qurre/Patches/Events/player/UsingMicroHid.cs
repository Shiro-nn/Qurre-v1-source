using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using System;
using System.Linq;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(MicroHID), nameof(MicroHID.UpdateServerside))]
    internal static class UsingMicroHid
    {
        private static bool Prefix(MicroHID __instance)
        {
            try
            {
                if (!NetworkServer.active)
                    return false;
                if (__instance.refHub.inventory.curItem == ItemType.MicroHID)
                {
                    try
                    {
                        if (__instance.GetEnergy() != __instance.Energy)
                            __instance.ChangeEnergy(__instance.Energy);
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                    foreach (var item in __instance.refHub.inventory.items.Where(x => x.id == ItemType.MicroHID))
                        __instance.NetworkEnergy = item.durability;

                if (__instance.keyAntiSpamCooldown > 0f) __instance.keyAntiSpamCooldown -= Time.deltaTime;

                if (__instance.refHub.inventory.curItem == ItemType.MicroHID || __instance.chargeup > 0f)
                {
                    if (__instance.CurrentHidState != MicroHID.MicroHidState.Idle)
                    {
                        __instance.refHub.weaponManager.scp268.ServerDisable();
                        __instance._visionController.MakeNoise(__instance.CurrentHidState == MicroHID.MicroHidState.Discharge ? 20 : 75);
                    }

                    MicroHID.MicroHidState state = MicroHID.MicroHidState.Idle;

                    if (__instance.refHub.inventory.curItem == ItemType.MicroHID)
                    {
                        if (__instance.Energy > 0f && __instance.chargeup >= 1f && __instance.SyncKeyCode == 2)
                            state = MicroHID.MicroHidState.Discharge;
                        else if (__instance.Energy > 0f && __instance.chargeup < 1f && __instance.SyncKeyCode != 0 && __instance.CurrentHidState != MicroHID.MicroHidState.RampDown)
                            state = MicroHID.MicroHidState.RampUp;
                        else if (__instance.chargeup > 0f && (__instance.SyncKeyCode == 0 || __instance.Energy <= 0f || __instance.CurrentHidState == MicroHID.MicroHidState.RampDown))
                            state = MicroHID.MicroHidState.RampDown;
                        else if (__instance.chargeup <= 0f && (__instance.SyncKeyCode == 0 || __instance.Energy <= 0f || __instance.CurrentHidState == MicroHID.MicroHidState.RampDown))
                            state = MicroHID.MicroHidState.Idle;
                        else if (__instance.chargeup >= 1f)
                            state = MicroHID.MicroHidState.Spinning;
                    }
                    else state = MicroHID.MicroHidState.RampDown;

                    var player = Player.Get(__instance.refHub);
                    var item = player.ItemInfoInHand;

                    var ev = new MicroHidUsingEvent(player, item, state);
                    Qurre.Events.Invoke.Player.MicroHidUsing(ev);
                    if (!ev.Allowed) ev.State = MicroHID.MicroHidState.Idle;
                    state = ev.State;

                    switch (state)
                    {
                        case MicroHID.MicroHidState.Discharge:
                            if (__instance.soundEffectPause >= 1f)
                            {
                                __instance.NetworkEnergy = Mathf.Clamp01(__instance.Energy - Time.deltaTime * __instance.dischargeEnergyLoss);
                                __instance.DealDamage();
                            }
                            else __instance.NetworkEnergy = Mathf.Clamp01(__instance.Energy - Time.deltaTime * __instance.speedBasedEnergyLoss.Evaluate(1f));
                            break;
                        case MicroHID.MicroHidState.RampUp:
                            __instance.chargeup = Mathf.Clamp01(__instance.chargeup + Time.deltaTime / __instance.chargeupTime);
                            __instance.NetworkEnergy = Mathf.Clamp01(__instance.Energy - Time.deltaTime * __instance.speedBasedEnergyLoss.Evaluate(__instance.chargeup));
                            break;
                        case MicroHID.MicroHidState.RampDown:
                            __instance.chargeup = Mathf.Clamp01(__instance.chargeup - Time.deltaTime / __instance.chargedownTime);
                            break;
                        case MicroHID.MicroHidState.Spinning:
                            __instance.NetworkEnergy = Mathf.Clamp01(__instance.Energy - Time.deltaTime * __instance.speedBasedEnergyLoss.Evaluate(__instance.chargeup));
                            break;
                    }
                    __instance.NetworkCurrentHidState = state;
                }
                if (__instance.Energy <= 0.05f) __instance.NetworkEnergy = 0f;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [UsingMicroHid]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}