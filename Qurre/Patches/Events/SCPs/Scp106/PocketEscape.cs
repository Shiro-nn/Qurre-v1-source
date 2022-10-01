using Achievements;
using CustomPlayerEffects;
using HarmonyLib;
using MapGeneration;
using Qurre.API.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Qurre.Patches.Events.SCPs.Scp106
{
    using Qurre.API;
    [HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.SuccessEscape))]
    internal static class PocketEscape
    {
        private static bool Prefix(PocketDimensionTeleport __instance, ReferenceHub hub)
        {
            try
            {
                if (!(hub.scp106PlayerScript.GrabbedPosition == Vector3.zero))
                {
                    _ = hub.scp106PlayerScript.GrabbedPosition;
                }
                else
                {
                    hub.scp106PlayerScript.GrabbedPosition = RoomIdentifier.AllRoomIdentifiers.ElementAt(Random.Range(0, RoomIdentifier.AllRoomIdentifiers.Count - 1)).transform.position;
                }

                RoomIdentifier roomIdentifier = RoomIdUtils.RoomAtPosition(hub.scp106PlayerScript.GrabbedPosition);
                if (roomIdentifier.Zone == FacilityZone.Surface)
                {
                    ReferenceHub referenceHub = null;
                    foreach (ReferenceHub value in ReferenceHub.GetAllHubs().Values)
                    {
                        if (value.characterClassManager.CurClass == RoleType.Scp106)
                        {
                            referenceHub = hub;
                            break;
                        }
                    }

                    Vector3 a = ((referenceHub == null) ? Vector3.zero : referenceHub.playerMovementSync.RealModelPosition);
                    SafeTeleportPosition componentInChildren = roomIdentifier.GetComponentInChildren<SafeTeleportPosition>();
                    float num = Vector3.Distance(a, componentInChildren.SafePositions[0].position);
                    float num2 = Vector3.Distance(a, componentInChildren.SafePositions[1].position);
                    var ev = new PocketEscapeEvent(Player.Get(hub), (num2 < num) ? componentInChildren.SafePositions[0].position : componentInChildren.SafePositions[1].position);
                    Qurre.Events.Invoke.Scp106.PocketEscape(ev);
                    if (!ev.Allowed) return false;
                    hub.playerMovementSync.OverridePosition(ev.TeleportPosition, new PlayerMovementSync.PlayerRotation(null, Random.value * 360f));
                }
                else
                {
                    HashSet<RoomIdentifier> hashSet = RoomIdUtils.FindRooms(RoomName.Unnamed, roomIdentifier.Zone, RoomShape.Undefined);
                    while (hashSet.Count > 0)
                    {
                        RoomIdentifier roomIdentifier2 = hashSet.ElementAt(Random.Range(0, hashSet.Count));
                        Vector3 position = roomIdentifier2.transform.position;
                        SafeTeleportPosition componentInChildren2 = roomIdentifier2.GetComponentInChildren<SafeTeleportPosition>();
                        if (componentInChildren2 != null && componentInChildren2.SafePositions.Length != 0)
                        {
                            position = componentInChildren2.SafePositions[Random.Range(0, componentInChildren2.SafePositions.Length - 1)].position;
                        }

                        if (PlayerMovementSync.FindSafePosition(position, out var safePos))
                        {
                            var ev = new PocketEscapeEvent(Player.Get(hub), safePos);
                            Qurre.Events.Invoke.Scp106.PocketEscape(ev);
                            if (!ev.Allowed) return false;
                            safePos = ev.TeleportPosition;
                            hub.playerMovementSync.OverridePosition(safePos, new PlayerMovementSync.PlayerRotation(null, Random.value * 360f));
                            break;
                        }

                        hashSet.Remove(roomIdentifier2);
                    }
                }

                hub.playerEffectsController.EnableEffect<Disabled>(10f, addDurationIfActive: true);
                hub.playerEffectsController.GetEffect<Corroding>().Intensity = 0;
                AchievementHandlerBase.ServerAchieve(hub.networkIdentity.connectionToClient, AchievementName.LarryFriend);
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