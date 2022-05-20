using System;
using HarmonyLib;
using InventorySystem.Items.Usables;
using Qurre.API.Controllers;
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
                    Item item = Item.Get(msg.ItemSerial);
                    if (item is null) return true;
                    var ev = new ItemUsingEvent(item.Owner, item);
                    Qurre.Events.Invoke.Player.ItemUsing(ev);
                    return ev.Allowed;
                }
                else
                {
                    Item item = Item.Get(msg.ItemSerial);
                    if (item is null) return true;
                    var ev = new ItemStoppingEvent(item.Owner, item);
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