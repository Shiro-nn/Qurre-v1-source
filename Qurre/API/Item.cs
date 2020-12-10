namespace Qurre.API
{
    public static class Item
    {
        public static void WeaponAmmo(this Inventory.SyncListItemInfo list, Inventory.SyncItemInfo item, int amount) => list.ModifyDuration(list.IndexOf(item), amount);
        public static void WeaponAmmo(this ReferenceHub player, Inventory.SyncItemInfo item, int amount) => player.inventory.items.ModifyDuration(player.inventory.items.IndexOf(item), amount);
        public static float WeaponAmmo(this Inventory.SyncItemInfo item) => item.durability;
    }
}