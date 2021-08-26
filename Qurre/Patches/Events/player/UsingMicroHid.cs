using HarmonyLib;
using InventorySystem.Items.MicroHID;
using Qurre.API;
using Qurre.API.Events;
using System;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(MicroHIDItem), nameof(MicroHIDItem.ExecuteServerside))]
    internal static class UsingMicroHid
    {
        private static bool Prefix(MicroHIDItem __instance)
        {
            try
            {
                var state = __instance.State;
                var energyToByte = __instance.EnergyToByte;
                var num = 0f;
                var player = Player.Get(__instance.Owner);
                var ev = new MicroHidUsingEvent(player, __instance, state);
                Qurre.Events.Invoke.Player.MicroHidUsing(ev);
                state = ev.State;
                if (!ev.Allowed) state = HidState.Idle;
                switch (state)
                {
                    case HidState.Idle:
                        if (__instance.RemainingEnergy > 0f && __instance.UserInput != HidUserInput.None)
                        {
                            __instance.State = HidState.PoweringUp;
                            __instance._stopwatch.Restart();
                        }
                        break;

                    case HidState.PoweringUp:
                        if ((__instance.UserInput == HidUserInput.None && __instance._stopwatch.Elapsed.TotalSeconds >= 0.35) || __instance.RemainingEnergy <= 0f)
                        {
                            __instance.State = HidState.PoweringDown;
                            __instance._stopwatch.Restart();
                        }
                        else if (__instance._stopwatch.Elapsed.TotalSeconds >= 5.95)
                        {
                            __instance.State = ((__instance.UserInput == HidUserInput.Fire) ? HidState.Firing : HidState.Primed);
                            __instance._stopwatch.Restart();
                        }

                        num = __instance._energyConsumtionCurve.Evaluate((float)(__instance._stopwatch.Elapsed.TotalSeconds / 5.95));
                        break;

                    case HidState.PoweringDown:
                        if (__instance._stopwatch.Elapsed.TotalSeconds >= 3.1)
                        {
                            __instance.State = HidState.Idle;
                            __instance._stopwatch.Stop();
                            __instance._stopwatch.Reset();
                        }
                        break;

                    case HidState.Primed:
                        if ((__instance.UserInput != HidUserInput.Prime && __instance._stopwatch.Elapsed.TotalSeconds >= 0.34999999403953552) || __instance.RemainingEnergy <= 0f)
                        {
                            __instance.State = ((__instance.UserInput == HidUserInput.Fire && __instance.RemainingEnergy > 0f) ? HidState.Firing : HidState.PoweringDown);
                            __instance._stopwatch.Restart();
                        }
                        else num = __instance._energyConsumtionCurve.Evaluate(1f);
                        break;

                    case HidState.Firing:
                        if (__instance._stopwatch.Elapsed.TotalSeconds > 1.7)
                        {
                            num = 0.13f;
                            __instance.Fire();
                            if (__instance.RemainingEnergy == 0f || (__instance.UserInput != HidUserInput.Fire && __instance._stopwatch.Elapsed.TotalSeconds >= 2.05))
                            {
                                __instance.State = ((__instance.RemainingEnergy > 0f && __instance.UserInput == HidUserInput.Prime) ? HidState.Primed : HidState.PoweringDown);
                                __instance._stopwatch.Restart();
                            }
                        }
                        else num = __instance._energyConsumtionCurve.Evaluate(1f);
                        break;
                }
                if (state != __instance.State) __instance.ServerSendStatus(HidStatusMessageType.State, (byte)__instance.State);

                if (num > 0f)
                {
                    __instance.RemainingEnergy = Mathf.Clamp01(__instance.RemainingEnergy - num * Time.deltaTime * ev.Coefficient);
                    if (energyToByte != __instance.EnergyToByte) __instance.ServerSendStatus(HidStatusMessageType.EnergySync, __instance.EnergyToByte);
                }
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