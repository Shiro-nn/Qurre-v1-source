using HarmonyLib;
using Qurre.API;
using System;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
    [HarmonyPatch(typeof(Scp173PlayerScript), "LookFor173", new Type[] { typeof(bool), typeof(Scp173PlayerScript), typeof(GameObject) })]
    internal static class Scp173Controller
    {
        private static void Postfix(ref bool __result, Scp173PlayerScript __instance, GameObject scp)
        {
            try
            {
                var player = Player.Get(__instance.gameObject);
                var _scp = Player.Get(scp);
                if (!__result) return;
                if (player.Team == Team.SCP && !Plugin.Config.GetBool("Qurre_ScpTrigger173", false))
                    __result = false;
                if (_scp.Role == RoleType.Scp173 && _scp.Scp173Controller.IgnoredPlayers.Contains(player))
                    __result = false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Controllers [Scp173Controller]:\n{e}\n{e.StackTrace}");
            }
        }
    }
}