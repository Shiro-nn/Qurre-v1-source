using HarmonyLib;
using Mirror;
using PlayerStatsSystem;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
namespace Qurre.Patches.Controllers
{
	[HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.ServerSpawnRagdoll))]
	internal static class RagdollController
	{
		private static bool Prefix(ReferenceHub hub, DamageHandlerBase handler)
		{
			try
			{
				if (!NetworkServer.active) return false;
				if (hub is null) return false;
				GameObject model_ragdoll = hub.characterClassManager.CurRole.model_ragdoll;
				if (model_ragdoll is null) return false;
				if (!Object.Instantiate(model_ragdoll).TryGetComponent(out Ragdoll component)) return false;
				var ragdoll = new API.Controllers.Ragdoll(component, hub.playerId);
				Map.Ragdolls.Add(ragdoll);
				var ev = new RagdollSpawnEvent(Player.Get(hub), ragdoll);
				Qurre.Events.Invoke.Player.RagdollSpawn(ev);
				if (ev.Allowed is false) return false;
				component.NetworkInfo = new RagdollInfo(hub, handler, model_ragdoll.transform.localPosition, model_ragdoll.transform.localRotation);
				NetworkServer.Spawn(component.gameObject);
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