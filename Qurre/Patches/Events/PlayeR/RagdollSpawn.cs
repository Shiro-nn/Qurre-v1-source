#pragma warning disable SA1313
using System;
using HarmonyLib;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Events.PlayeR
{
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.SpawnRagdoll))]
    internal static class RagdollSpawn
    {
        private static bool Prefix(RagdollManager __instance, ref Vector3 pos, ref Quaternion rot, ref int classId, ref PlayerStats.HitInfo ragdollInfo,
            ref bool allowRecall, ref string ownerID, ref string ownerNick, ref int playerId)
        {
            try
            {
                var ev = new RagdollSpawnEvent(ragdollInfo.PlayerId == 0 ? null : ReferenceHub.GetHub(ragdollInfo.PlayerId),
                    ReferenceHub.GetHub(__instance.gameObject), pos, rot, (RoleType)classId, ragdollInfo, allowRecall, ownerID, ownerNick, playerId);
                Qurre.Events.Player.ragdollSpawn(ev);
                pos = ev.Position;
                rot = ev.Rotation;
                classId = (int)ev.RoleType;
                ragdollInfo = ev.HitInfo;
                allowRecall = ev.IsRecallAllowed;
                ownerID = ev.DissonanceId;
                ownerNick = ev.PlayerNickname;
                playerId = ev.PlayerId;
                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching PlayeR.RagdollSpawn:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}