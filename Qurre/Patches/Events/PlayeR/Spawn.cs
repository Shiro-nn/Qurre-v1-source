#pragma warning disable SA1313
using HarmonyLib;
using Mirror;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ApplyProperties))]
    internal class Spawn
    {
        private static bool Prefix(CharacterClassManager __instance, bool lite = false, bool escape = false)
        {
            try
            {
                Role role = __instance.Classes.SafeGet(__instance.CurClass);
                if (!__instance._wasAnytimeAlive() && __instance.CurClass != RoleType.Spectator && __instance.CurClass != RoleType.None)
                    __instance._wasAnytimeAlive(true);
                __instance.InitSCPs();
                __instance.AliveTime = 0f;
                switch (role.team)
                {
                    case Team.MTF:
                        break;
                    case Team.CHI:
                        break;
                    case Team.RSC:
                    case Team.CDP:
                        __instance.EscapeStartTime = (int)Time.realtimeSinceStartup;
                        break;
                }
                __instance.GetComponent<Inventory>();
                try { __instance.GetComponent<FootstepSync>().SetLoudness(role.team, role.roleId.Is939()); } catch { }
                if (NetworkServer.active)
                {
                    Handcuffs hcs = __instance.GetComponent<Handcuffs>();
                    hcs.ClearTarget();
                    hcs.NetworkCufferId = -1;
                    Respawning.SpawnableTeamType spawnableTeamType;
                    string[] array;
                    if (role.roleId != RoleType.Spectator && 
                        Respawning.RespawnManager.CurrentSequence() != Respawning.RespawnManager.RespawnSequencePhase.SpawningSelectedTeam && 
                        Respawning.NamingRules.UnitNamingManager.RolesWithEnforcedDefaultName.TryGetValue(role.roleId, out spawnableTeamType) && 
                        Respawning.RespawnManager.Singleton.NamingManager.TryGetAllNamesFromGroup((byte)spawnableTeamType, out array) && array.Length != 0)
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
                if (role.team != Team.RIP)
                {
                    if (NetworkServer.active && !lite)
                    {
                        
                        Vector3 cRP = NonFacilityCompatibility.currentSceneSettings.constantRespawnPoint;
                        if (cRP != Vector3.zero) __instance._pms().OnPlayerClassChange(cRP, 0f);
                        else
                        {
                            GameObject GO = _spawnpointManager().GetRandomPosition(__instance.CurClass);
                            Vector3 sP;
                            float rotY;
                            if (GO != null)
                            {
                                sP = GO.transform.position;
                                rotY = GO.transform.rotation.eulerAngles.y;
                                AmmoBox AB = __instance.GetComponent<AmmoBox>();
                                if (escape && CharacterClassManager.KeepItemsAfterEscaping)
                                {
                                    Inventory inv = PlayerManager.localPlayer.GetComponent<Inventory>();
                                    for (ushort index = 0; index < 3; ++index)
                                        if (AB[index] >= 15U)
                                            inv.SetPickup(AB.types[index].inventoryID, AB[index], GO.transform.position, GO.transform.rotation, 0, 0, 0);
                                }
                                AB.ResetAmmo();
                            }
                            else
                            {
                                sP = __instance.DeathPosition;
                                rotY = 0f;
                            }
                            var ev = new SpawnEvent(API.Player.Get(__instance.gameObject), __instance.CurClass, sP, rotY);
                            Qurre.Events.Player.spawn(ev);
                            __instance._pms().OnPlayerClassChange(ev.Position, ev.RotationY);
                        }
                        if (!__instance.SpawnProtected && CharacterClassManager.EnableSP && CharacterClassManager.SProtectedTeam.Contains((int)role.team))
                        {
                            __instance.GodMode = true;
                            __instance.SpawnProtected = true;
                            __instance.ProtectedTime = Time.time;
                        }
                    }
                    if (!__instance.isLocalPlayer)
                        API.Player.Get(__instance.gameObject).MaxHP = role.maxHP;
                }
                __instance.Scp0492.iAm049_2 = __instance.CurClass == RoleType.Scp0492;
                __instance.Scp106.iAm106 = __instance.CurClass == RoleType.Scp106;
                __instance.Scp173.iAm173 = __instance.CurClass == RoleType.Scp173;
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