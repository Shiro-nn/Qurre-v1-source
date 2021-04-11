using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.SCPs.Scp173
{
    [HarmonyPatch(typeof(Scp173PlayerScript), "FixedUpdate")]
    internal static class Blink
    {
        private static bool Prefix(Scp173PlayerScript __instance)
        {
            try
            {
                if ((Scp173_remainingTime - Time.fixedDeltaTime) >= 0f) return true;
                if (!API.Round.IsStarted) return true;
                __instance.Scp173_DoBlinkingSequence();
                if (!__instance.iAm173 || (!__instance.isLocalPlayer && !NetworkServer.active)) return false;
                HashSet<Player> players = new HashSet<Player>();
                foreach (GameObject gameObject in PlayerManager.players)
                {
                    Scp173PlayerScript component = gameObject.GetComponent<Scp173PlayerScript>();
                    if (!component.SameClass && component.Scp173_LookFor173(__instance.gameObject, true) && __instance.Scp173_LookFor173(component.gameObject, false))
                        players.Add(Player.Get(gameObject));
                }
                var scp = Player.Get(__instance.gameObject);
                scp.Scp173Controller.ConfrontingPlayers = players;
                if (scp.Scp173Controller.ConfrontingPlayers.Count != 0)
                {
                    var ev = new BlinkEvent(scp, players);
                    Qurre.Events.SCPs.SCP173.blink(ev);
                }
                __instance.Scp173_AllowMove(true);
                foreach (GameObject gameObject in PlayerManager.players)
                {
                    Scp173PlayerScript component = gameObject.GetComponent<Scp173PlayerScript>();
                    if (!component.SameClass && component.Scp173_LookFor173(__instance.gameObject, true) && __instance.Scp173_LookFor173(component.gameObject, false))
                    {
                        __instance.Scp173_AllowMove(false);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> Scp173 [Blink]:\n{e}\n{e.StackTrace}");
            }
            return false;
        }
    }
}