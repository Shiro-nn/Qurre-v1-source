using System;
using InventorySystem.Items.Usables.Scp330;
using Qurre.API;
using HarmonyLib;
using Qurre.API.Events;
using InventorySystem;
using System.Collections.Generic;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup))]
    internal class PickupCandy
    {
        internal static bool Prefix(ReferenceHub ply, Scp330Pickup pickup, out Scp330Bag bag, ref bool __result)
        {
            try
            {
                if (!Scp330Bag.TryGetBag(ply, out bag))
                {
                    int num = (!(pickup == null)) ? pickup.Info.Serial : 0;
                    __result = ply.inventory.ServerAddItem(ItemType.SCP330, (ushort)num, pickup) != null;
                    return false;
                }
                List<CandyKindID> CandyList = new();
                if (pickup is null) CandyList.Add(Scp330Candies.GetRandom());
                else
                {
                    while (pickup.StoredCandies.Count > 0 && 6 > bag.Candies.Count)
                    {
                        CandyList.Add(pickup.StoredCandies[0]);
                        pickup.StoredCandies.RemoveAt(0);
                    }
                }
                var ev = new PickupCandyEvent(Player.Get(ply), CandyList);
                Qurre.Events.Invoke.Player.PickupCandy(ev);
                if (!ev.Allowed)
                {
                    __result = false;
                    return false;
                }
                foreach (var candy in ev.Candy) bag.TryAddSpecific(candy);
                if (bag.AcquisitionAlreadyReceived) bag.ServerRefreshBag();

                __result = ev.Candy.Count > 0;
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [CandyPickup]:\n{e}\n{e.StackTrace}");
                try { Scp330Bag.TryGetBag(ply, out bag); } catch { bag = null; }
                return true;
            }
        }
    }
}