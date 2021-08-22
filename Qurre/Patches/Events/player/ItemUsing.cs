using System;
using HarmonyLib;
using InventorySystem.Items.Usables;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.ServerReceivedStatus))]
    internal static class ItemUsingPatch
    {
        private static bool Prefix(StatusMessage msg)
        {
            try
            {
                if (msg.Status == StatusMessage.StatusType.Start)
                {
                    var ev = new ItemUsingEvent(msg.ItemSerial.GetItem().Holder, msg.ItemSerial.GetItem());
                    Qurre.Events.Invoke.Player.ItemUsing(ev);
                    return ev.Allowed;
                }
                else
                {
                    var ev = new ItemStoppingEvent(msg.ItemSerial.GetItem().Holder, msg.ItemSerial.GetItem());
                    Qurre.Events.Invoke.Player.ItemStopping(ev);
                    return ev.Allowed;
                }
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [ItemUsing]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}