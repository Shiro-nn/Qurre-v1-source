using Mirror;
using UnityEngine;
namespace Qurre.API
{
    public static class Item
    {
        public static Pickup Spawn(ItemType itemType, float durability, Vector3 position, Quaternion rotation = default, int sight = 0, int barrel = 0, int other = 0)
            => Server.InventoryHost.SetPickup(itemType, durability, position, rotation, sight, barrel, other);
		public static GameObject Spawn(ItemType itemType, Vector3 position, Vector3 rotation, Vector3 scale)
		{
			Pickup yesnt = PlayerManager.localPlayer.GetComponent<Inventory>().SetPickup(itemType, -4.656647E+11f, position, Quaternion.identity, 0, 0, 0);

			GameObject gameObject = yesnt.gameObject;
			gameObject.transform.localScale = scale;
			gameObject.transform.eulerAngles = rotation;

			NetworkServer.UnSpawn(gameObject);
			NetworkServer.Spawn(yesnt.gameObject);
			return yesnt.gameObject;
		}
		public static void WeaponAmmo(this Inventory.SyncListItemInfo list, Inventory.SyncItemInfo item, int amount) => list.ModifyDuration(list.IndexOf(item), amount);
        public static void WeaponAmmo(this ReferenceHub player, Inventory.SyncItemInfo item, int amount) => player.inventory.items.ModifyDuration(player.inventory.items.IndexOf(item), amount);
        public static float WeaponAmmo(this Inventory.SyncItemInfo item) => item.durability;
    }
}