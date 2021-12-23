using HarmonyLib;
using InventorySystem.Items.Coin;
using Mirror;
using Qurre.API;
using Qurre.API.Events;
using UnityEngine;
using Utils.Networking;
using static InventorySystem.Items.Coin.CoinNetworkHandler;
namespace Qurre.Patches.Events.player
{
    [HarmonyPatch(typeof(CoinNetworkHandler), nameof(CoinNetworkHandler.ServerProcessMessage))]
    internal static class CoinFlip
    {
        internal static bool Prefix(NetworkConnection conn)
        {
            try
            {
                if(!((ReferenceHub.TryGetHub(conn.identity.gameObject, out ReferenceHub hub) && hub.inventory.CurItem.TypeId == ItemType.Coin))) return false;
                var ev = new CoinFlipEvent(Player.Get(hub), Random.value >= 0.5f);
                Qurre.Events.Invoke.Player.CoinFlip(ev);
                if(!ev.Allowed) return false;
                NetworkUtils.SendToAuthenticated(new CoinFlipMessage(hub.inventory.CurItem.SerialNumber, ev.Tails));
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"umm, error in patching Player [CoinFlip]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}