#pragma warning disable SA1313
using System.Linq;
using Scp914;
using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API;
using static Qurre.API.Events.SCP914;
namespace Qurre.Patches.Events.SCPs.SCP914
{
    [HarmonyPatch(typeof(Scp914Machine), nameof(Scp914Machine.ProcessItems))]
    internal static class Upgrade
    {
        private static bool Prefix(Scp914Machine __instance)
        {
            try
            {
                if (!NetworkServer.active)
                    return true;
                Collider[] colliderArray = Physics.OverlapBox(__instance.intake.position, __instance.inputSize / 2f);
                QurreModLoader.umm.Scp914_players(__instance).Clear();
                QurreModLoader.umm.Scp914_items(__instance).Clear();
                foreach (Collider collider in colliderArray)
                {
                    CharacterClassManager plrs = collider.GetComponent<CharacterClassManager>();
                    if (plrs != null)
                        QurreModLoader.umm.Scp914_players(__instance).Add(plrs);
                    else
                    {
                        Pickup picks = collider.GetComponent<Pickup>();
                        if (picks != null)
                            QurreModLoader.umm.Scp914_items(__instance).Add(picks);
                    }
                }
                var ev = new UpgradeEvent(__instance, QurreModLoader.umm.Scp914_players(__instance).Select(player => Player.Get(player.gameObject)).ToList(), QurreModLoader.umm.Scp914_items(__instance), __instance.knobState);
                Qurre.Events.SCPs.SCP914.upgrade(ev);
                var players = ev.Players.Select(player => player.characterClassManager).ToList();
                __instance.MoveObjects(ev.Items, players);
                if (ev.IsAllowed)
                    __instance.UpgradeObjects(ev.Items, players);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs.SCP914.Upgrade:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}