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
                if (curRole.team != Team.RIP)
                {
                    if (NetworkServer.active && !lite)
                    {
                        float rotY = 0f;
                        Vector3 pos = NonFacilityCompatibility.currentSceneSettings.constantRespawnPoint;
                        if (pos != Vector3.zero) __instance._pms.IsAFK = true;
                        else
                        {
                            GameObject randomPosition = CharacterClassManager._spawnpointManager.GetRandomPosition(__instance.CurClass);
                            if (randomPosition != null)
                            {
                                pos = randomPosition.transform.position;
                                rotY = randomPosition.transform.rotation.eulerAngles.y;
                                __instance._pms.IsAFK = true;
                            }
                            else pos = __instance.DeathPosition;
                        }
                        var ev = new SpawnEvent(API.Player.Get(__instance.gameObject), __instance.CurClass, pos, rotY);
                        Qurre.Events.Invoke.Player.Spawn(ev);
                        __instance._pms.OnPlayerClassChange(ev.Position, ev.RotationY);
                        if (!__instance.SpawnProtected && CharacterClassManager.EnableSP && CharacterClassManager.SProtectedTeam.Contains((int)curRole.team))
                        {
                            __instance.GodMode = true;
                            __instance.SpawnProtected = true;
                            __instance.ProtectedTime = Time.time;
                        }
                    }
                    if (!__instance.isLocalPlayer) API.Player.Get(__instance.gameObject).MaxHp = curRole.maxHP;
                }
                __instance.Scp0492.iAm049_2 = __instance.CurClass == RoleType.Scp0492;
                __instance.Scp106.iAm106 = __instance.CurClass == RoleType.Scp106;
                __instance.Scp939.iAm939 = __instance.CurClass.Is939();
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