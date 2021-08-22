using System;
using HarmonyLib;
using Mirror;
using Qurre.API.Events;
using Qurre.Events.Invoke;
using UnityEngine;
using static CharacterClassManager;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
    internal static class RoleChange
    {
        private static bool Prefix(CharacterClassManager __instance, RoleType classid, GameObject ply, SpawnReason spawnReason, bool lite = false)
        {
            try
            {
                if (!NetworkServer.active) return false;
                API.Player pl = API.Player.Get(ply);
                if (pl.ReferenceHub.isDedicatedServer || !pl.ReferenceHub.Ready) return false;
                var ev = new RoleChangeEvent(pl, classid, lite, spawnReason);
                Player.RoleChange(ev);
                if (!ev.Allowed) return false;
                if (ev.NewRole == RoleType.Spectator) ev.Player.DropItems();
                lite = ev.SavePos;
                spawnReason = ev.Reason;
                if (classid != RoleType.Spectator && ev.NewRole == RoleType.Spectator)
                    Player.Dead(new DeadEvent(API.Server.Host, ev.Player, new PlayerStats.HitInfo(-1, "Dedicated Server", DamageTypes.None, 0, true)));
                classid = ev.NewRole;
                pl.ClassManager.SetClassIDAdv(classid, lite, spawnReason, false);
                ply.GetComponent<FirstPersonController>().ResetStamina();
                pl.PlayerStats.SetHPAmount(__instance.Classes.SafeGet(classid).maxHP);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [RoleChange]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}