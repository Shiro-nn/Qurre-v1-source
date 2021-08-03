using HarmonyLib;
using Qurre.API;
using System;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Scp173PlayerScript), "LookFor173")]
    internal static class Scp173Controller
    {
        private static bool Prefix(bool __result, Scp173PlayerScript __instance, GameObject scp)
        {
            try
            {
                var player = Player.Get(__instance.gameObject);
                var _scp = Player.Get(scp);
                if (!__result) return true;
                if (player.Team == Team.SCP && !Plugin.Config.GetBool("Qurre_ScpTrigger173", false))
                    return false;
                if (_scp.Role == RoleType.Scp173 && _scp.Scp173Controller.IgnoredPlayers.Contains(player))
                    return false;
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [Scp173Controller]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}