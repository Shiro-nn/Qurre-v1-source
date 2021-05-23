#pragma warning disable SA1313
using HarmonyLib;
using UnityEngine;
using Qurre.API.Events;
namespace Qurre.Patches.Events.SCPs.SCP096
{
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.AddTarget))]
    static class AddTargetLook
    {
        public static bool Prefix(PlayableScps.Scp096 __instance, GameObject target)
        {
            try
            {
                if (target == null) return true;
                API.Player targetPL = API.Player.Get(target);
                API.Player player = API.Player.Get(__instance.Hub.gameObject);
                var ev = new AddTargetEvent(__instance, player, targetPL);
                Qurre.Events.SCPs.SCP096.addtarget(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [AddTargetLook]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.OnDamage))]
    static class AddTargetShoot
    {
        public static bool Prefix(PlayableScps.Scp096 __instance, PlayerStats.HitInfo info)
        {
            try
            {
                if (__instance == null || __instance.Hub == null || info == null) return true;
                API.Player player = API.Player.Get(__instance.Hub.gameObject);
                API.Player target = API.Player.Get(info.RHub);
                if ((player != null && player.Invisible) || (target != null && target.Invisible)) return false;
                var ev = new AddTargetEvent(__instance, player, target);
                Qurre.Events.SCPs.SCP096.addtarget(ev);
                return ev.Allowed;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [AddTargetShoot]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}