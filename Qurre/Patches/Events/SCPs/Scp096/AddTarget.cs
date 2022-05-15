using HarmonyLib;
using UnityEngine;
using Qurre.API.Events;
using PlayerStatsSystem;
using Mirror;
using PlayableScps;
using PlayableScps.Messages;

namespace Qurre.Patches.Events.SCPs.Scp096
{
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.AddTarget))]
    internal static class AddTargetLook
    {
        public static bool Prefix(PlayableScps.Scp096 __instance, GameObject target)
        {
            try
            {
                if (!NetworkServer.active) return false;
                if (target is null) return true;
                ReferenceHub hub = ReferenceHub.GetHub(target);
                if (__instance.CanReceiveTargets && hub is not null && !__instance._targets.Contains(hub))
                {
                    if (!CollectionExtensions.IsEmpty(__instance._targets) || 
                        __instance.PlayerState is Scp096PlayerState.Docile or Scp096PlayerState.TryNotToCry || __instance.Enraging)
                    {
                        __instance.AddReset();
                    }

                    var ev = new AddTargetEvent(__instance, API.Player.Get(__instance.Hub.gameObject), API.Player.Get(hub));
                    Qurre.Events.Invoke.Scp096.AddTarget(ev);
                    if (!ev.Allowed) return false;

                    if (CollectionExtensions.IsEmpty(__instance._targets))
                    {
                        __instance.Hub.characterClassManager.netIdentity.connectionToClient.Send(new Scp096ToTargetMessage(__instance.Hub));
                    }
                    __instance._targets.Add(hub);
                    hub.characterClassManager.netIdentity.connectionToClient.Send(new Scp096ToTargetMessage(hub));
                }
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [AddTargetLook]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
    [HarmonyPatch(typeof(PlayableScps.Scp096), nameof(PlayableScps.Scp096.OnDamage))]
    internal static class AddTargetShoot
    {
        public static bool Prefix(PlayableScps.Scp096 __instance, DamageHandlerBase handler)
        {
            try
            {
                if (handler is not AttackerDamageHandler dodo) return true;
                if (dodo.Attacker.Hub is null) return true;
                if (__instance.CanEnrage) return true;
                API.Player player = API.Player.Get(__instance.Hub.gameObject);
                API.Player target = API.Player.Get(dodo.Attacker.Hub);
                var ev = new AddTargetEvent(__instance, player, target);
                Qurre.Events.Invoke.Scp096.AddTarget(ev);
                __instance.AddTarget(dodo.Attacker.Hub.gameObject);
                __instance.Windup();
                __instance.Shield.SustainTime = 25f;
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP096 [AddTargetShoot]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}