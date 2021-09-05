using HarmonyLib;
using Scp914;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.SCPs.Scp914
{
    [HarmonyPatch(typeof(Scp914Controller), nameof(Scp914Controller.ServerInteract))]
    internal static class Interact
    {
        private static bool Prefix(Scp914Controller __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                if (__instance._remainingCooldown > 0f) return false;
                switch ((Scp914InteractCode)colliderId)
                {
                    case Scp914InteractCode.ChangeMode:
                        Scp914KnobSetting scp914KnobSetting = __instance._knobSetting + 1;
                        if (!Enum.IsDefined(typeof(Scp914KnobSetting), scp914KnobSetting)) scp914KnobSetting = Scp914KnobSetting.Rough;
                        var ev = new KnobChangeEvent(API.Player.Get(ply), scp914KnobSetting);
                        Qurre.Events.Invoke.Scp914.KnobChange(ev);
                        if (!ev.Allowed) return false;
                        __instance._remainingCooldown = __instance._knobChangeCooldown;
                        __instance.Network_knobSetting = ev.Setting;
                        __instance.RpcPlaySound(0);
                        break;
                    case Scp914InteractCode.Activate:
                        var _ev = new ActivatingEvent(API.Player.Get(ply), __instance._totalSequenceTime);
                        Qurre.Events.Invoke.Scp914.Activating(_ev);
                        if (!_ev.Allowed) return false;
                        __instance._remainingCooldown = _ev.Duration;
                        __instance._isUpgrading = true;
                        __instance._itemsAlreadyUpgraded = false;
                        __instance.RpcPlaySound(1);
                        break;
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [Interact]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}