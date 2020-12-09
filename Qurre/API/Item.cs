namespace Qurre.API
{
    public static class Item
    {
        public static void SetWeaponAmmo(this Inventory.SyncListItemInfo list, Inventory.SyncItemInfo item, int amount) => list.ModifyDuration(list.IndexOf(item), amount);
        public static void SetWeaponAmmo(this ReferenceHub player, Inventory.SyncItemInfo item, int amount) => player.inventory.items.ModifyDuration(player.inventory.items.IndexOf(item), amount);
        public static float GetWeaponAmmo(this Inventory.SyncItemInfo item) => item.durability;
    }
}