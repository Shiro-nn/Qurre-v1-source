using System;
using HarmonyLib;
using InventorySystem.Items.Firearms.BasicMessages;
using Qurre.API;
using Qurre.API.Controllers;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerRequestReceived))]
    internal static class RechargeWeapon
    {
        private static bool Prefix(RequestMessage msg)
        {
            try
            {
                Item item = Item.Get(msg.Serial);
                if (item == null) return true;
                if (msg.Request == RequestType.AdsIn) item.Owner.Zoomed = true;
                else if (msg.Request == RequestType.AdsOut) item.Owner.Zoomed = false;
                else if (msg.Request == RequestType.Reload)
                {
                    Player pl = item.Owner;
                    var ev = new RechargeWeaponEvent(pl, item, msg.Request);
                    Qurre.Events.Invoke.Player.RechargeWeapon(ev);
                    return ev.Allowed;
                }
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [RechargeWeapon]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}