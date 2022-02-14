using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactables.Interobjects;
using InventorySystem.Searching;
using InventorySystem.Items.Usables.Scp330; 
using HarmonyLib;
using Footprinting;
using UnityEngine;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{ 
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]
    internal class InteractSCP330
    { 
        internal static bool Prefix(Scp330Interobject __instance, ReferenceHub ply, byte colliderId)
        {
            if (!ply.characterClassManager.IsHuman()) return false;

            Footprint footprint = new(ply);

            float num = 0.1f;
            int candies = 0;

            var ev = new InteractScp330Event(Player.Get(ply));
            Qurre.Events.Invoke.Player.InteractScp330(ev); 
            foreach(Footprint footpr in __instance._takenCandies.Where(x => x.Equals(footprint)))
            {
                num = Mathf.Min(num, (float)footpr.Stopwatch.Elapsed.TotalSeconds);
                candies++; 
            } 
            if (num < 0.1f) return false;
            __instance.RpcMakeSound(); 
            if(candies > 2)
            {
                ply.playerEffectsController.EnableEffect<CustomPlayerEffects.SeveredHands>(0f, false);   
                while(__instance._takenCandies.Remove(footprint)) 
                    return false; 
            } 
            if(!Scp330Bag.ServerProcessPickup(ply, null, out var bag)) 
            {
                Scp330SearchCompletor.ShowOverloadHint(ply, bag != null); 
            }
            __instance._takenCandies.Add(footprint);
            return false; 
        }
    }
}
