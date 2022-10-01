using System;
using HarmonyLib;
using InventorySystem.Items.Firearms.BasicMessages;
using Qurre.API.Controllers;
using Qurre.API.Events;
namespace Qurre.Patches.Events.Player
{
    [HarmonyPatch(typeof(FirearmBasicMessagesHandler), nameof(FirearmBasicMessagesHandler.ServerRequestReceived))]
    internal static class RechargeWeapon
    {
        private static bool Prefix(RequestMessage msg)
        {
            try
            {
                Item item = Item.Get(msg.Serial);
                if (item is null) return true;
                switch (msg.Request)
                {
                    case RequestType.AdsIn:
                        {
                            Qurre.Events.Invoke.Player.Zooming(new(item.Owner, item, msg.Request, true));
                            item.Owner.Zoomed = true;
                            return true;
                        }
                    case RequestType.AdsOut:
                        {
                            Qurre.Events.Invoke.Player.Zooming(new(item.Owner, item, msg.Request, false));
                            item.Owner.Zoomed = false;
                            return true;
                        }
                    case RequestType.Reload:
                        {
                            var ev = new RechargeWeaponEvent(item.Owner, item, msg.Request);
                            Qurre.Events.Invoke.Player.RechargeWeapon(ev);
                            return ev.Allowed;
                        }
                    default: return true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [RechargeWeapon]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}