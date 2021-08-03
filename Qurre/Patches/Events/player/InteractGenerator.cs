using System;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using QurreModLoader;
using UnityEngine;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(Generator079), nameof(Generator079.Interact))]
	internal static class InteractGenerator
	{
		private static bool Prefix(Generator079 __instance, GameObject person, PlayerInteract.Generator079Operations command)
		{
			try
			{
				Player player = Player.Get(person);
				if (player == null)
					return true;
				if (__instance.Gen_doorAnimationCooldown() > 0f || __instance.Gen_deniedCooldown() > 0f)
					return false;
				switch (command)
				{
					case (PlayerInteract.Generator079Operations)PlayerInteract.Generator079Operations.Door:
						bool boolean = false;
                        if (__instance.GetGenerator().Locked)
						{
							if (player.Inventory.curItem > ItemType.KeycardJanitor)
							{
								string[] permissions = player.Inventory.GetItemByID(player.Inventory.curItem).permissions;
								for (int i = 0; i < permissions.Length; i++)
									if (permissions[i] == "ARMORY_LVL_2" || player.BypassMode)
										boolean = true;
							}
							var ev1 = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.Unlocked, boolean);
							Qurre.Events.Invoke.Player.InteractGenerator(ev1);
							if (ev1.Allowed) __instance.GetGenerator().Locked = false;
							else __instance.RpcDenied();
						}
                        else
						{
							switch (__instance.isDoorOpen)
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
							if (boolean) __instance.OpenClose(person);
							else __instance.RpcDenied();
						}
						break;
					case (PlayerInteract.Generator079Operations)PlayerInteract.Generator079Operations.Tablet:
						if (__instance.isTabletConnected || !__instance.isDoorOpen || __instance.Generator_localTime() <= 0.0 || Generator079.mainGenerator.forcedOvercharge)
							break;
						Inventory component = person.GetComponent<Inventory>();
						using (SyncList<Inventory.SyncItemInfo>.SyncListEnumerator enumerator = component.items.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								Inventory.SyncItemInfo current = enumerator.Current;
								if (current.id == ItemType.WeaponManagerTablet)
								{
									var ev1 = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.TabletInjected);
									Qurre.Events.Invoke.Player.InteractGenerator(ev1);
									if (ev1.Allowed)
									{
										component.items.Remove(current);
										__instance.NetworkisTabletConnected = true;
									}
									break;
								}
							}
							break;
						}
					case (PlayerInteract.Generator079Operations)PlayerInteract.Generator079Operations.Cancel:
						var ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.TabledEjected);
						Qurre.Events.Invoke.Player.InteractGenerator(ev);
						if (ev.Allowed) __instance.EjectTablet();
						break;
				}
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