using CustomPlayerEffects;
using HarmonyLib;
using Qurre.API.Events;
using Qurre.Events.Invoke;
using System;
namespace Qurre.Patches.Events.Effects
{
    [HarmonyPatch(typeof(PlayerEffect), nameof(PlayerEffect.Intensity), MethodType.Setter)]
    internal static class ChangeIntensity
    {
        private static bool Prefix(PlayerEffect __instance, ref byte value)
        {
            try
            {
                if (!(__instance._intensity != value && (__instance.AllowEnabling || value <= __instance._intensity))) return false;
                byte intensity = __instance._intensity;
                if (intensity == 0 && value > 0)
                {
                    var pl = API.Player.Get(__instance.Hub);
                    var ev = new EffectEnabledEvent(pl, __instance);
                    Effect.Enabled(ev);
                    if (!ev.Allowed) return false;
                }
                else if (intensity > 0 && 0 >= value)
                {
                    var pl = API.Player.Get(__instance.Hub);
                    var ev = new EffectDisabledEvent(pl, __instance);
                    Effect.Disabled(ev);
                    if (!ev.Allowed) return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Effects [ChangeIntensity]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}