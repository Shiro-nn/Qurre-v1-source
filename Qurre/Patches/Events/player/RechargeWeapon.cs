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
                if (msg.Request == RequestType.AdsIn)
                {
                    Player pl = item.Owner;
                    var ev = new ZoomingEvent(pl, item, msg.Request, true);
                    Qurre.Events.Invoke.Player.Zooming(ev);
                    if (ev.Allowed) item.Owner.Zoomed = true;
                    return ev.Allowed;
                }
                else if (msg.Request == RequestType.AdsOut)
                {
                    Player pl = item.Owner;
                    var ev = new ZoomingEvent(pl, item, msg.Request, false);
                    Qurre.Events.Invoke.Player.Zooming(ev);
                    if (ev.Allowed) item.Owner.Zoomed = false;
                    return ev.Allowed;
                }
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