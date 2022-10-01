using System;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Distributors;
using Qurre.API.Events;
using Qurre.API.Objects;
namespace Qurre.Patches.Events.Player
{
	using Qurre.API;
	[HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
	internal static class InteractGenerator
	{
		private static bool Prefix(Scp079Generator __instance, ReferenceHub ply, byte colliderId)
		{
			try
			{
				Player player = Player.Get(ply);
				if (player == null) return true;
				if (__instance._cooldownStopwatch.IsRunning && __instance._cooldownStopwatch.Elapsed.TotalSeconds < __instance._targetCooldown) return false;
				if (colliderId != 0 && !__instance.HasFlag(__instance._flags, Scp079Generator.GeneratorFlags.Open)) return false;
				__instance._cooldownStopwatch.Stop();
				switch (colliderId)
				{
					case 0:
						if (__instance.HasFlag(__instance._flags, Scp079Generator.GeneratorFlags.Unlocked))
						{
							bool boolean = false;
							switch (__instance.GetGenerator().Open)
							{
								case false:
									var ev1 = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.OpenDoor);
									Qurre.Events.Invoke.Player.InteractGenerator(ev1);
									boolean = ev1.Allowed;
									break;
								case true:
									var ev2 = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.CloseDoor);
									Qurre.Events.Invoke.Player.InteractGenerator(ev2);
									boolean = ev2.Allowed;
									break;
							}
							if (boolean) __instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Open, !__instance.HasFlag(__instance._flags, Scp079Generator.GeneratorFlags.Open));
							else __instance.RpcDenied();
							__instance._targetCooldown = __instance._doorToggleCooldownTime;
						}
						else
						{
							bool boolean = false;
							InventorySystem.Items.Keycards.KeycardItem keycardItem;
							if (ply.inventory.CurInstance != null && (keycardItem = ply.inventory.CurInstance as InventorySystem.Items.Keycards.KeycardItem) != null &&
								keycardItem.Permissions.HasFlagFast(__instance._requiredPermission))
								boolean = true;
							var _ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.Unlocked, boolean);
							Qurre.Events.Invoke.Player.InteractGenerator(_ev);
							if (_ev.Allowed) __instance.GetGenerator().Locked = false;
							else __instance.RpcDenied();
							__instance._targetCooldown = __instance._unlockCooldownTime;
						}
						break;
					case 1:
						if ((ply.characterClassManager.IsHuman() || __instance.Activating) && !__instance.Engaged)
						{
							var gen = __instance.GetGenerator();
							var ev1 = new InteractGeneratorEvent(player, gen, gen.Active ? GeneratorStatus.Disabled : GeneratorStatus.Activated);
							Qurre.Events.Invoke.Player.InteractGenerator(ev1);
							if (ev1.Allowed)
							{
								__instance.Activating = !__instance.Activating;
								if (__instance.Activating) __instance._leverStopwatch.Restart();
								__instance._targetCooldown = __instance._doorToggleCooldownTime;
							}
						}
						break;
					case 2:
						if (__instance.Activating && !__instance.Engaged)
						{
							var ev1 = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.Disabled);
							Qurre.Events.Invoke.Player.InteractGenerator(ev1);
							if (ev1.Allowed)
							{
								__instance.ServerSetFlag(Scp079Generator.GeneratorFlags.Activating, false);
								__instance._targetCooldown = __instance._unlockCooldownTime;
							}
						}
						break;
					default:
						__instance._targetCooldown = 1f;
						break;
				}
				__instance._cooldownStopwatch.Restart();
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching PlayeR.InteractGenerator:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}