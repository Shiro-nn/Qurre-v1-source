using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Items.Usables.Scp330; 
using InventorySystem.Items.Pickups;
using Qurre.API;
using HarmonyLib;
using Mirror;
using Qurre.API.Events;

namespace Qurre.Patches.Events.player
{ 
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup))]
    internal class CandyPickup 
    { 
        internal static void Prefix(Scp330Bag __instance, ReferenceHub ply, ItemPickupBase pickup, ICandy candy) 
        {
            try
            { 
                var ev = new CandyPickupEvent(Player.Get(ply), candy); 
                Qurre.Events.Invoke.Player.CandyPickup(ev);
                __instance.OnAdded(pickup);   
                if (!NetworkServer.active) return;
                if (!Scp330Bag.ServerProcessPickup(ply, pickup as Scp330Pickup, out var bag)) return;  
                if (bag == null || bag == __instance) return; 
                __instance.ServerRemoveSelf(); 
                
            } 
            catch (Exception e)
            {
                Log.Error($"ummmm, error in patching Player[CandyPickup]:{e}\n{e.StackTrace}");
            }
        }
    }
}
