using System;
using HarmonyLib;
using InventorySystem.Items.Firearms.BasicMessages;
using Qurre.API;
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
                if (msg.Serial.GetItem() == null) return true;
                var ev = new RechargeWeaponEvent(msg.Serial.GetItem().Holder, msg.Serial.GetItem(), msg.Request);
                Qurre.Events.Invoke.Player.RechargeWeapon(ev);
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [RechargeWeapon]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}