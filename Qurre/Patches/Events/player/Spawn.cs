using HarmonyLib;
using Mirror;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ApplyProperties))]
    internal static class Spawn
    {
        private static bool Prefix(CharacterClassManager __instance, bool lite = false, bool escape = false)
        {
            try
            {
                try { if (__instance == null || __instance.gameObject == null) return true; } catch { return true; }
                Role curRole = __instance.CurRole;
                if (!__instance._wasAnytimeAlive && __instance.CurClass != RoleType.Spectator && __instance.CurClass != RoleType.None) __instance._wasAnytimeAlive = true;
                __instance.InitSCPs();
                __instance.AliveTime = 0f;
                Team team = curRole.team;
                if (team - Team.RSC <= 1) __instance.EscapeStartTime = (int)Time.realtimeSinceStartup;
                try { __instance._hub.footstepSync.SetLoudness(curRole.team, curRole.roleId.Is939()); } catch { }
                if (NetworkServer.active)
                {
                    if (curRole.roleId != RoleType.Spectator &&
                        Respawning.RespawnManager.CurrentSequence() != Respawning.RespawnManager.RespawnSequencePhase.SpawningSelectedTeam &&
                        Respawning.NamingRules.UnitNamingManager.RolesWithEnforcedDefaultName.TryGetValue(curRole.roleId, out Respawning.SpawnableTeamType spawnableTeamType) &&
                        Respawning.RespawnManager.Singleton.NamingManager.TryGetAllNamesFromGroup((byte)spawnableTeamType, out string[] array) && array.Length != 0)
                    {
                        __instance.NetworkCurSpawnableTeamType = (byte)spawnableTeamType;
                        __instance.NetworkCurUnitName = array[0];
                    }
                    else if (__instance.CurSpawnableTeamType != 0)
                    {
                        __instance.NetworkCurSpawnableTeamType = 0;
                        __instance.NetworkCurUnitName = string.Empty;
                    }
                }
                var __pl = API.Player.Get(__instance.gameObject);
                if (curRole.team != Team.RIP)
                {
                    if (NetworkServer.active && !lite)
                    {
                        float rotY = 0f;
                        Vector3 pos = NonFacilityCompatibility.currentSceneSettings.constantRespawnPoint;
                        if (pos != Vector3.zero) __instance._pms.IsAFK = true;
                        else
                        {
                            var randomPosition = API.Map.GetRandomSpawnTransform(__instance.CurClass);
                            if (randomPosition != null)
                            {
                                pos = randomPosition.position;
                                rotY = randomPosition.rotation.eulerAngles.y;
                                __instance._pms.IsAFK = true;
                            }
                            else pos = __instance.DeathPosition;
                        }
                        if (__pl == null) __pl = API.Server.Host;
                        else __pl.Zoomed = false;
                        var ev = new SpawnEvent(__pl, __instance.CurClass, pos, rotY);
                        Qurre.Events.Invoke.Player.Spawn(ev);
                        if (!ev.Player.BlockSpawnTeleport) __instance._pms.OnPlayerClassChange(ev.Position, ev.RotationY);
                        else
                        {
                            ev.Player.BlockSpawnTeleport = false;
                            __instance._pms._successfullySpawned = true;
                        }
                        if (!__instance.SpawnProtected && CharacterClassManager.EnableSP && CharacterClassManager.SProtectedTeam.Contains((int)curRole.team))
                        {
                            __instance.GodMode = true;
                            __instance.SpawnProtected = true;
                            __instance.ProtectedTime = Time.time;
                        }
                        try { if (!__instance.isLocalPlayer) __pl.MaxHp = curRole.maxHP; } catch { }
                        try { __pl.Hp = __pl.MaxHp; } catch { }
                    }
                }
                else if (__pl != null)
                {
                    __pl.Zoomed = false;
                    __pl.BlockSpawnTeleport = false;
                }
                __instance.Scp0492.iAm049_2 = __instance.CurClass == RoleType.Scp0492;
                __instance.Scp106.iAm106 = __instance.CurClass == RoleType.Scp106;
                __instance.RefreshPlyModel();
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [Spawn]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}