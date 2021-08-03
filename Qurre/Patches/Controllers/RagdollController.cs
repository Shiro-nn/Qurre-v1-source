using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using static QurreModLoader.umm;
namespace Qurre.Patches.Controllers
{
	[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.SpawnRagdoll))]
	internal static class RagdollController
	{
		private static bool Prefix(RagdollManager __instance, Vector3 pos, Quaternion rot, Vector3 velocity, int classId, PlayerStats.HitInfo ragdollInfo, bool allowRecall, string ownerID, string ownerNick, int playerId)
		{
			try
			{
				Role role = __instance.RagdollHub().characterClassManager.Classes.SafeGet(classId);
				if (role.model_ragdoll == null) return false;
				var gameObject = Object.Instantiate(role.model_ragdoll, pos + role.ragdoll_offset.position, Quaternion.Euler(rot.eulerAngles + role.ragdoll_offset.rotation));
				NetworkServer.Spawn(gameObject);
				Ragdoll component = gameObject.GetComponent<Ragdoll>();
				component.Networkowner = new Ragdoll.Info(ownerID, ownerNick, ragdollInfo, role, playerId);
				component.NetworkallowRecall = allowRecall;
				component.NetworkPlayerVelo = velocity;
				var ragdoll = new API.Controllers.Ragdoll(component);
				Map.Ragdolls.Add(ragdoll);
				var ev = new RagdollSpawnEvent(ragdollInfo.PlayerId == 0 ? null : API.Player.Get(ragdollInfo.PlayerId), API.Player.Get(__instance.gameObject), ragdoll);
				Qurre.Events.Invoke.Player.RagdollSpawn(ev);
				if (!ev.Allowed) ragdoll.Destroy();
				return false;
			}
			catch (System.Exception e)
			{
				Log.Error($"umm, error in patching Controllers [RagdollController]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}