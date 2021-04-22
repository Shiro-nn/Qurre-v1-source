﻿#pragma warning disable SA1313
using System.Linq;
using Scp914;
using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API;
using Qurre.API.Events;
using static QurreModLoader.umm;
using System.Collections.Generic;

namespace Qurre.Patches.Events.SCPs.SCP914
{
    [HarmonyPatch(typeof(Scp914Machine), nameof(Scp914Machine.ProcessItems))]
    internal static class Upgrade
    {
        private static bool Prefix(Scp914Machine __instance)
        {
            try
            {
                if (!NetworkServer.active) return true;
                if (__instance.intake == null || __instance.inputSize == null) return true;
                Collider[] colliderArray = Physics.OverlapBox(__instance.intake.position, __instance.inputSize / 2f);
                __instance.Scp914_players().Clear();
                __instance.Scp914_items().Clear();
                foreach (Collider collider in colliderArray)
                {
                    try
                    {
                        if (collider == null) continue;
                        CharacterClassManager plrs = collider.GetComponent<CharacterClassManager>();
                        if (plrs != null) __instance.Scp914_players().Add(plrs);
                        else
                        {
                            Pickup picks = collider.GetComponent<Pickup>();
                            if (picks != null) __instance.Scp914_items().Add(picks);
                        }
                    }
                    catch { }
                }
                List<Player> _players = __instance.Scp914_players().Select(player => Player.Get(player.gameObject)).ToList();
                foreach (Player pl in _players)
                {
                    var _ev = new UpgradePlayerEvent(__instance, pl, __instance.knobState);
                    Qurre.Events.SCPs.SCP914.upgradePlayer(_ev);
                    if (!_ev.Allowed) _players.Remove(pl);
                }
                var ev = new UpgradeEvent(__instance, _players, __instance.Scp914_items(), __instance.knobState);
                Qurre.Events.SCPs.SCP914.upgrade(ev);
                var players = ev.Players.Select(player => player.ClassManager).ToList();
                __instance.MoveObjects(ev.Items, players);
                if (ev.Allowed) __instance.UpgradeObjects(ev.Items, players);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP914 [Upgrade]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}