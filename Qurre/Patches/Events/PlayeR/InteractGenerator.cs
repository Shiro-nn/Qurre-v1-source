using System;
using System.Linq;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using UnityEngine;
namespace Qurre.Patches.Events.PlayeR
{
	[HarmonyPatch(typeof(Generator079), nameof(Generator079.Interact))]
	internal static class InteractGenerator
	{
		private static bool Prefix(Generator079 __instance, GameObject person, PlayerInteract.Generator079Operations command)
		{
			try
			{
				var player = Player.Get(person);
				switch (command)
				{
					case (PlayerInteract.Generator079Operations)PlayerInteract.Generator079Operations.Tablet:
						if (__instance.isTabletConnected || !__instance.isDoorOpen || QurreModLoader.umm.Generator_localTime(__instance) <= 0f || Generator079.mainGenerator.forcedOvercharge)
							return false;
						Inventory inv = person.GetComponent<Inventory>();
						using (SyncList<Inventory.SyncItemInfo>.SyncListEnumerator SLE = inv.items.GetEnumerator())
						{
							while (SLE.MoveNext())
							{
								Inventory.SyncItemInfo inf = SLE.Current;
								if (inf.id == ItemType.WeaponManagerTablet)
								{
									var ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.TabletInjected);
									Qurre.Events.Player.interactGenerator(ev);
									if (ev.Allowed) __instance.isTabletConnected = ev.Allowed;
									break;
								}
							}
						}
						return false;
					case (PlayerInteract.Generator079Operations)PlayerInteract.Generator079Operations.Cancel:
						if (!__instance.isTabletConnected) return false;
						var ev1 = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.TabledEjected);
						Qurre.Events.Player.interactGenerator(ev1);
						return ev1.Allowed;
				}
				return true;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching PlayeR.InteractGenerator:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
	[HarmonyPatch(typeof(Generator079), nameof(Generator079.OpenClose))]
	public static class InteractGeneratorDoor
	{
		public static bool Prefix(Generator079 __instance, GameObject person)
		{
			try
			{
				var player = Player.Get(person);
				if (player == null) return false;
				if (player.Inventory == null || QurreModLoader.umm.Gen_doorAnimationCooldown(__instance) > 0f || QurreModLoader.umm.Gen_deniedCooldown(__instance) > 0f) return false;
				if (__instance.isDoorUnlocked)
				{
					var allow = true;
					if (!__instance.isDoorOpen)
					{
						var ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.OpenDoor);
						Qurre.Events.Player.interactGenerator(ev);
						allow = ev.Allowed;
					}
					else
					{
						var ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.CloseDoor);
						Qurre.Events.Player.interactGenerator(ev);
						allow = ev.Allowed;
					}
					if (!allow)
					{
						__instance.RpcDenied();
						return false;
					}
					__instance.isDoorOpen = !__instance.isDoorOpen;
					return false;
				}
				var boolean = player.BypassMode;
				if (player.Inventory.GetItemInHand().id > ItemType.KeycardJanitor)
				{
					var permissions = player.Inventory.GetItemByID(player.Inventory.curItem).permissions;
					foreach (var t in permissions.Where(x => x == "ARMORY_LVL_2")) boolean = true;
				}
				var ev1 = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.TabledEjected, boolean);
				Qurre.Events.Player.interactGenerator(ev1);
				if (ev1.Allowed)
				{
					__instance.isDoorUnlocked = true;
					return false;
				}
				__instance.RpcDenied();
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching PlayeR.InteractGeneratorDoor:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}