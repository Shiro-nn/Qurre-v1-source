using HarmonyLib;
using InventorySystem;
using Qurre.API.Events;
using Qurre.API.Objects;
using System;
namespace Qurre.Patches.Events.Player
{
    using Qurre.API;
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropAmmo))]
    internal static class DropAmmo
    {
        private static bool Prefix(Inventory __instance, ref byte ammoType, ref ushort amount)
        {
            try
            {
                Player pl = Player.Get(__instance._hub);
                AmmoType type = ((ItemType)ammoType).GetAmmoType();
                var ev = new DropAmmoEvent(pl, type, amount);
                Qurre.Events.Invoke.Player.DropAmmo(ev);
                ammoType = (byte)ev.Type.GetItemType();
                amount = ev.Amount;
                return ev.Allowed;
            }
            catch (Exception e)
            {
                Log.Error($"umm, error in patching Player [DropAmmo]:\n{e}\n{e.StackTrace}");
                return true;
            }
        }
    }
}