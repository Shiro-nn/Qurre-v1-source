using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    using Qurre.API;
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdContain106))]
    internal static class Contain
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            bool allowed = !(!__instance.CanInteract || !Object.FindObjectOfType<LureSubjectContainer>().allowContain ||
                (__instance._ccm.CurRole.team == Team.SCP && __instance._ccm.CurClass != RoleType.Scp106) ||
                !__instance.ChckDis(GameObject.FindGameObjectWithTag("FemurBreaker").transform.position) ||
                OneOhSixContainer.used || __instance._ccm.CurRole.team == Team.RIP);
            if (allowed) allowed = Player.List.TryFind(out _, x => x.Role is RoleType.Scp106 && !x.GodMode);
            var ev = new ContainEvent(Player.Get(__instance._hub), allowed);
            Qurre.Events.Invoke.Scp106.Contain(ev);
            return ev.Allowed;
        }
    }
}