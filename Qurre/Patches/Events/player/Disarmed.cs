using HarmonyLib;
using InventorySystem;
using InventorySystem.Disarming;
using Qurre.API;
using Qurre.API.Events;
using System;
namespace Qurre.Patches.Events.player
{
	[HarmonyPatch(typeof(DisarmedPlayers), nameof(DisarmedPlayers.SetDisarmedStatus))]
	internal static class DisarmedPatch
	{
		private static bool Prefix(this Inventory inv, Inventory disarmer)
		{
			try
			{
				if (inv == null || disarmer == null) return true;
				Player target = Player.Get(inv._hub);
				Player cuffer = Player.Get(disarmer._hub);
                if (!DisarmedPlayers.IsDisarmed(inv))
				{
					var ev = new CuffEvent(cuffer, target);
					Qurre.Events.Invoke.Player.Cuff(ev);
					return ev.Allowed;
                }
                else
				{
					var ev = new UnCuffEvent(cuffer, target);
					Qurre.Events.Invoke.Player.UnCuff(ev);
					return ev.Allowed;
				}
			}
			catch (Exception e)
			{
				Log.Error($"umm, error in patching Player [Disarmed]:\n{e}\n{e.StackTrace}");
				return true;
			}
		}
	}
}