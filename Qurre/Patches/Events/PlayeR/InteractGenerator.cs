using System;
using HarmonyLib;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using Qurre.API.Objects;
using QurreModLoader;
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
						if (__instance.isTabletConnected || !__instance.isDoorOpen || __instance.Generator_localTime() <= 0f || Generator079.mainGenerator.forcedOvercharge)
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
			Player player = Player.Get(person);
			if (player == null)
				return true;
			if (__instance.Gen_doorAnimationCooldown() > 0f || __instance.Gen_deniedCooldown() > 0f)
				return false;
			try
			{
				if (__instance.isDoorUnlocked)
				{
					InteractGeneratorEvent ev;
					if (__instance.NetworkisDoorOpen) ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.CloseDoor);
					else ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.OpenDoor);
					Qurre.Events.Player.interactGenerator(ev);
					if (!ev.Allowed) return false;
					__instance.Gen_doorAnimationCooldown(1.5f);
					__instance.NetworkisDoorOpen = !__instance.isDoorOpen;
					__instance.Generator_RpcDoSound(__instance.isDoorOpen);
					return false;
				}

				if (player.Inventory.curItem > ItemType.KeycardJanitor)
				{
					string[] permissions = player.Inventory.GetItemByID(player.Inventory.curItem).permissions;
					for (int i = 0; i < permissions.Length; i++)
					{
						if (permissions[i] == "ARMORY_LVL_2" || player.BypassMode)
						{
							var ev = new InteractGeneratorEvent(player, __instance.GetGenerator(), GeneratorStatus.Unlocked);
							Qurre.Events.Player.interactGenerator(ev);
							if (!ev.Allowed) return false;
							__instance.NetworkisDoorUnlocked = true;
							__instance.Gen_doorAnimationCooldown(0.5f);
							return false;
						}
					}
				}
				__instance.RpcDenied();
				return false;
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [InteractGeneratorDoor]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}