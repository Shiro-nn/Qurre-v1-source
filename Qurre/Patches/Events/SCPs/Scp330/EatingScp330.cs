using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventorySystem.Searching;
using InventorySystem.Items.Usables.Scp330;
using Qurre.API.Events;
using Qurre.API;
using HarmonyLib; 
namespace Qurre.Patches.Events.player
{ 
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.ServerOnUsingCompleted))]
    internal class EatingScp330
    { 
        internal static void Prefix(Scp330Bag __instance, ReferenceHub ply, ICandy candy)
        {
            try
            {
                var ev = new EatingScp330Event(Player.Get(ply), candy);
                Qurre.Events.Invoke.Player.EatingScp330(ev);
                if (!__instance.IsCandySelected) return;
                if (!Scp330Candies.CandiesById.TryGetValue(__instance.Candies[__instance.SelectedCandyId], out candy)) return;        
                __instance.IsUsing = false; 
                candy.ServerApplyEffects(ply);
                __instance.Candies.RemoveAt(__instance.SelectedCandyId);
                ply.inventory.ServerSelectItem(0);
                __instance.ServerRefreshBag(); 
           
            } 
            catch(Exception e)
            {
                Log.Error($"ummmm, error in patching Player[EatingScp330]:{e}\n{ e.StackTrace}");
            }
        }
    }
}
