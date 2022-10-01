using System;
using HarmonyLib;
using Mirror;
using PlayerStatsSystem;
using Qurre.API.Events;
using Qurre.API.Objects;
using UnityEngine;
using static CharacterClassManager;
namespace Qurre.Patches.Events.Player
{
    using Qurre.Events.Invoke;
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
    internal static class RoleChange
    {
        private static bool Prefix(RoleType classid, GameObject ply, SpawnReason spawnReason, bool lite = false)
        {
            try
            {
                if (!NetworkServer.active) return false;
                ReferenceHub hub = ReferenceHub.GetHub(ply);
                if(hub.isDedicatedServer || !hub.Ready) return false;
                API.Player pl = API.Player.Get(hub);
                var ev = new RoleChangeEvent(pl, classid, lite, spawnReason);
                Player.RoleChange(ev);
                if (!ev.Allowed) return false;
                if (ev.NewRole == RoleType.Spectator) ev.Player.DropItems();
                lite = ev.SavePos;
                spawnReason = ev.Reason;
                if (classid != RoleType.Spectator && ev.NewRole == RoleType.Spectator)
                    Player.Dead(new DeadEvent(API.Server.Host, ev.Player, new UniversalDamageHandler(), DamageTypes.None));
                classid = ev.NewRole;
                hub.characterClassManager.SetClassIDAdv(classid, lite, spawnReason);
                ply.GetComponent<FirstPersonController>().ResetStamina();
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