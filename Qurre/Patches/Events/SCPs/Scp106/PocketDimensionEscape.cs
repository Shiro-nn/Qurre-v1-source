﻿using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Mirror;
using UnityEngine;
using Qurre.API.Events;
using MapGeneration;
using Qurre.API;
using Qurre.API.Objects;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
    internal static class PocketDimensionEscape
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
                        var ev = new PocketDimensionFailEscapeEvent(pl, __instance);
                        Qurre.Events.Invoke.Scp106.PocketDimensionFailEscape(ev);
                        if (!ev.Allowed) return false;
                    }
                    pl.Damage(9999, DamageTypes.Pocket);
                }
                else if (__instance._type == PocketDimensionTeleport.PDTeleportType.Exit)
                {
                    if (pl.Scp106PlayerScript.GrabbedPosition != Vector3.zero) _ = pl.Scp106PlayerScript.GrabbedPosition;
                    else pl.Scp106PlayerScript.GrabbedPosition = new Vector3(0f, -1997f, 0f);

                    RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(pl.Scp106PlayerScript.GrabbedPosition);
                    if (roomIdentifier.Zone == FacilityZone.Surface)
                    {
                        foreach (var value in Player.List)
                        {
                            if (value.Role == RoleType.Scp106)
                            {
                                float num = Vector3.Distance(value.Position, __instance._gateBPDPosition);
                                float num2 = Vector3.Distance(value.Position, __instance._gateAPDPosition);
                                var pos = (num2 < num) ? __instance._gateBPDPosition : __instance._gateAPDPosition;
                                var ev = new PocketDimensionEscapeEvent(pl, pos);
                                Qurre.Events.Invoke.Scp106.PocketDimensionEscape(ev);
                                if (!ev.Allowed) return false;
                                pos = ev.TeleportPosition;
                                pl.PlayerMovementSync.OverridePosition(pos, Random.value * 360f);
                                break;
                            }
                        }
                    }
                    else
                    {
                        HashSet<RoomIdentifier> hashSet = RoomIdUtils.FindRooms(RoomName.Unnamed, roomIdentifier.Zone, RoomShape.Undefined);
                        hashSet.RemoveWhere((RoomIdentifier room) => room.Name == RoomName.Hcz106 || room.Name == RoomName.EzGateA ||
                        room.Name == RoomName.EzGateB || (room.Zone == FacilityZone.LightContainment && room.Shape == RoomShape.Curve) ||
                        __instance.ProblemChildren.Contains(room.Name));
                        while (hashSet.Count > 0)
                        {
                            RoomIdentifier roomIdentifier2 = hashSet.ElementAt(Random.Range(0, hashSet.Count));
                            if (PlayerMovementSync.FindSafePosition(roomIdentifier2.transform.position, out Vector3 safePos))
                            {
                                var ev = new PocketDimensionEscapeEvent(pl, safePos);
                                Qurre.Events.Invoke.Scp106.PocketDimensionEscape(ev);
                                if (!ev.Allowed) return false;
                                safePos = ev.TeleportPosition;
                                pl.PlayerMovementSync.OverridePosition(safePos, Random.value * 360f);
                                break;
                            }

                            hashSet.Remove(roomIdentifier2);
                        }
                    }
                    pl.EnableEffect(EffectType.Disabled, 10f, addDurationIfActive: true);
                    pl.GetEffect(EffectType.Corroding).Intensity = 0;
                    pl.PlayerStats.TargetAchieve(component.connectionToClient, "larryisyourfriend");
                }
                ImageGenerator.pocketDimensionGenerator.GenerateRandom();
                foreach (var larry in Player.List.Where(x => x.Scp106Controller.PocketPlayers.Contains(pl)))
                    larry.Scp106Controller.PocketPlayers.Remove(pl);
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching SCPs -> SCP106 [PocketDimensionEscape]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}