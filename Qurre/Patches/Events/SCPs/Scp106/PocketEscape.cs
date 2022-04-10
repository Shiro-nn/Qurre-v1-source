using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
using MapGeneration;
using Qurre.API;
using Qurre.API.Objects;
using LightContainmentZoneDecontamination;
using Achievements;
using PlayerStatsSystem;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class PocketEscape
    {
        private static bool Prefix(PocketDimensionTeleport __instance, Collider other)
        {
            try
            {
                if (!NetworkServer.active) return false;

                var component = other.GetComponent<NetworkIdentity>();
                if (component == null) return false;
                var type = __instance._type;
                var pl = Player.Get(component?.gameObject);
                if (pl == null) return false;
                if (type == PocketDimensionTeleport.PDTeleportType.Killer || BlastDoor.OneDoor.isClosed)
                {
                    if (type == PocketDimensionTeleport.PDTeleportType.Killer)
                    {
                        var ev = new PocketFailEscapeEvent(pl, __instance);
                        Qurre.Events.Invoke.Scp106.PocketFailEscape(ev);
                        if (!ev.Allowed) return false;
                    }
                    pl.Damage(9999, DeathTranslations.PocketDecay);
                }
                else if (__instance._type == PocketDimensionTeleport.PDTeleportType.Exit)
                {
                    if (pl.Scp106PlayerScript.GrabbedPosition != Vector3.zero) _ = pl.Scp106PlayerScript.GrabbedPosition;
                    else
                    {
                        List<Vector3> tp = new();
                        bool flag = false;
                        DecontaminationController.DecontaminationPhase[] dP = DecontaminationController.Singleton.DecontaminationPhases;
                        if (DecontaminationController.GetServerTime > dP[dP.Length - 2].TimeTrigger) flag = true;
                        var _cfg = GameCore.ConfigFile.ServerConfig.GetStringList(flag ? "pd_random_exit_rids_after_decontamination" : "pd_random_exit_rids");
                        foreach (GameObject gO in GameObject.FindGameObjectsWithTag("RoomID"))
                        {
                            var _rid = gO.GetComponent<Rid>();
                            if (_rid != null && _cfg.Contains(_rid.id)) tp.Add(gO.transform.position);
                        }
                        if (_cfg.Contains("PORTAL"))
                            foreach (Scp106PlayerScript _script in Object.FindObjectsOfType<Scp106PlayerScript>())
                                if (_script.portalPosition != Vector3.zero)
                                    tp.Add(_script.portalPosition);
                        if (tp == null || tp.Count == 0)
                            foreach (GameObject _go in GameObject.FindGameObjectsWithTag("PD_EXIT"))
                                tp.Add(_go.transform.position);
                        pl.Scp106PlayerScript.GrabbedPosition = tp[Random.Range(0, tp.Count)];
                        pl.Scp106PlayerScript.GrabbedPosition.y += 2f;
                    }
                    RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(pl.Scp106PlayerScript.GrabbedPosition);
                    if (roomIdentifier.Zone == FacilityZone.Surface)
                    {
                        foreach (var value in Player.List)
                        {
                            if (value.Role == RoleType.Scp106)
                            {
                                SafeTeleportPosition componentInChildren = roomIdentifier.GetComponentInChildren<SafeTeleportPosition>();
                                float num = Vector3.Distance(value.Position, componentInChildren.SafePositions[0].position);
                                float num2 = Vector3.Distance(value.Position, componentInChildren.SafePositions[1].position);
                                var pos = (num2 < num) ? componentInChildren.SafePositions[0].position : componentInChildren.SafePositions[1].position;
                                var ev = new PocketEscapeEvent(pl, pos);
                                Qurre.Events.Invoke.Scp106.PocketEscape(ev);
                                if (!ev.Allowed) return false;
                                pos = ev.TeleportPosition;
                                pl.Movement.OverridePosition(pos, Random.value * 360f);
                                break;
                            }
                        }
                    }
                    else
                    {
                        HashSet<RoomIdentifier> hashSet = RoomIdUtils.FindRooms(RoomName.Unnamed, roomIdentifier.Zone, RoomShape.Undefined);
                        hashSet.RemoveWhere((RoomIdentifier room) => room.Name == RoomName.Hcz106 || room.Name == RoomName.EzGateA ||
                        room.Name == RoomName.EzGateB || (room.Zone == FacilityZone.LightContainment && room.Shape == RoomShape.Curve));
                        while (hashSet.Count > 0)
                        {
                            RoomIdentifier roomIdentifier2 = hashSet.ElementAt(Random.Range(0, hashSet.Count));
                            if (PlayerMovementSync.FindSafePosition(roomIdentifier2.transform.position, out Vector3 safePos))
                            {
                                var ev = new PocketEscapeEvent(pl, safePos);
                                Qurre.Events.Invoke.Scp106.PocketEscape(ev);
                                if (!ev.Allowed) return false;
                                safePos = ev.TeleportPosition;
                                pl.Movement.OverridePosition(safePos, Random.value * 360f);
                                break;
                            }

                            hashSet.Remove(roomIdentifier2);
                        }
                    }
                    pl.EnableEffect(EffectType.Disabled, 10f, addDurationIfActive: true);
                    pl.GetEffect(EffectType.Corroding).Intensity = 0;
                    AchievementHandlerBase.ServerAchieve(component.connectionToClient, AchievementName.LarryFriend);
                }
                ImageGenerator.pocketDimensionGenerator.GenerateRandom();
                foreach (var larry in Player.List.Where(x => x.Scp106Controller.PocketPlayers.Contains(pl)))
                    larry.Scp106Controller.PocketPlayers.Remove(pl);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP106 [PocketEscape]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}