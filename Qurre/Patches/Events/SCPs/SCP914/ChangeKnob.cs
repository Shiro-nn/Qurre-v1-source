#pragma warning disable SA1118
using System;
using HarmonyLib;
using Scp914;
using static Qurre.API.Events.SCP914;
namespace Qurre.Patches.Events.SCPs.SCP914
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdChange914Knob))]
    internal static class ChangeKnob
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            try
            {
                if (!QurreModLoader.umm.RateLimit(__instance).CanExecute(true) ||
                    (QurreModLoader.umm.InteractCuff(__instance).CufferId > 0 && !QurreModLoader.umm.DisarmedInteract()) ||
                    Scp914Machine.singleton.working || !__instance.ChckDis(Scp914Machine.singleton.knob.position) ||
                    Math.Abs(Scp914Machine.singleton.curKnobCooldown) > 0.001f)
                    return false;
                var ev = new ChangeKnobEvent(API.Player.Get(__instance.gameObject), Scp914Machine.singleton.knobState + 1);
                Qurre.Events.SCPs.SCP914.changeknob(ev);
                if (ev.IsAllowed)
                {
                    Scp914Machine.singleton.NetworkknobState = ev.KnobSetting;
                    Scp914Machine.singleton.curKnobCooldown = Scp914Machine.singleton.knobCooldown;
                    __instance.OnInteract();
                }
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP914.ChangeKnob:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}