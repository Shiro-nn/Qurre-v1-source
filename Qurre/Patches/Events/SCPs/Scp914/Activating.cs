using HarmonyLib;
using Scp914;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.SCPs.SCP914
{
    [HarmonyPatch(typeof(Scp914Controller), nameof(Scp914Controller.ServerInteract))]
    internal static class Activating
    {
        private static bool Prefix(Scp914Controller __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                if (__instance._remainingCooldown > 0f) return false;
                if (colliderId == 0)
                {
                    __instance._remainingCooldown = __instance._knobChangeCooldown;
                    Type typeFromHandle = typeof(Scp914KnobSetting);
                    Scp914KnobSetting scp914KnobSetting = __instance._knobSetting + 1;
                    __instance.Network_knobSetting = scp914KnobSetting;
                    if (!Enum.IsDefined(typeFromHandle, scp914KnobSetting)) __instance.Network_knobSetting = Scp914KnobSetting.Rough;
                    __instance.RpcPlaySound(0);
                    return false;
                }
                if (colliderId != 1) return false;
                var ev = new ActivatingEvent(API.Player.Get(ply), __instance._totalSequenceTime);
                Qurre.Events.Invoke.Scp914.Activating(ev);
                if (ev.Allowed)
                {
                    __instance._remainingCooldown = ev.Duration;
                    __instance._isUpgrading = true;
                    __instance._itemsAlreadyUpgraded = false;
                    __instance.RpcPlaySound(1);
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [Activating]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}