using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Radio;
using Mirror;
using Qurre.API.Objects;
using UnityEngine;
namespace Qurre.API.Controllers
{
    public class Item
    {
        public static Item None { get; } = new Item(-1);
        public static Dictionary<ushort, Item> AllItems { get; } = new Dictionary<ushort, Item>();
        private bool deactivated = false;
        public Item(ItemType type)
        {
            Serial = ItemSerialGenerator.GenerateNext();
            AllItems[Serial] = this;
            Id = (int)type;
            Name = type.ToString();
            CustomItem = false;
            Type = type;
            if (InventoryItemLoader.AvailableItems.TryGetValue(type, out var examplebase))
            {
                Category = examplebase.Category;
                TierFlags = examplebase.TierFlags;
                ThrowSettings = examplebase.ThrowSettings;
                Weight = examplebase.Weight;
            }
        }
        public Item(ItemType type, Player player) : this(type) => PickUp(player);
        public Item(ItemType type, Vector3 pos) : this(type) => Drop(pos);
        public Item(int id)
        {
            if (id == -1 && None == null)
            {
                Id = -1;
                Type = ItemType.None;
                Name = "None";
                return;
            }

            Serial = ItemSerialGenerator.GenerateNext();
            AllItems[Serial] = this;
            Id = id;

            if (id >= 0 && id <= Manager.Highest)
            {
                CustomItem = false;
                Type = (ItemType)id;
                Name = Type.ToString();
            }
            else
            {
                CustomItem = true;
                Type = Manager.GetBaseType(id);
                Name = Manager.GetName(id);
            }

            if (InventoryItemLoader.AvailableItems.TryGetValue(Type, out var examplebase))
            {
                Category = examplebase.Category;
                TierFlags = examplebase.TierFlags;
                ThrowSettings = examplebase.ThrowSettings;
                Weight = examplebase.Weight;
            }
        }
        public Item(int id, Player player) : this(id) => PickUp(player);
        public Item(int id, Vector3 pos) : this(id) => Drop(pos);
        public Item(ItemBase itemBase)
        {
            Serial = itemBase.OwnerInventory.UserInventory.Items.FirstOrDefault(x => x.Value == itemBase).Key;
            AllItems[Serial] = this;
            Id = (int)itemBase.ItemTypeId;
            Name = itemBase.ItemTypeId.ToString();
            CustomItem = false;
            Type = itemBase.ItemTypeId;
            Category = itemBase.Category;
            TierFlags = itemBase.TierFlags;
            ThrowSettings = ItemBase.ThrowSettings;
            Weight = itemBase.Weight;

            ItemBase = itemBase;
        }
        public Item(ItemPickupBase pickupBase)
        {
            Serial = pickupBase.Info.Serial;
            AllItems[Serial] = this;
            Id = (int)pickupBase.Info.ItemId;
            Name = pickupBase.Info.ItemId.ToString();
            CustomItem = false;
            Type = pickupBase.Info.ItemId;
            if (InventoryItemLoader.AvailableItems.TryGetValue(Type, out var examplebase))
            {
                Category = examplebase.Category;
                TierFlags = examplebase.TierFlags;
                ThrowSettings = ThrowSettings;
            }
            Weight = pickupBase.Info.Weight;
            PickupBase = pickupBase;
        }

        public readonly int Id;
        public readonly string Name;
        public readonly bool CustomItem;
        public readonly ItemType Type;
        public readonly ItemCategory Category;
        public ItemTierFlags TierFlags { get; }
        public ushort Serial { get; }
        public ItemThrowSettings ThrowSettings { get; set; }
        public float Weight { get; }

        public GameObject GameObject => ItemBase == null ? PickupBase.gameObject : ItemBase.gameObject;
        public ItemBase ItemBase { get; internal set; }
        public ItemPickupBase PickupBase { get; internal set; }

        public ItemState State
        {
            get
            {
                if (deactivated) return ItemState.Destroyed;
                if (ItemBase != null) return ItemState.Inventory;
                if (PickupBase != null) return ItemState.Map;
                return ItemState.Despawned;
            }
        }

        public Player Holder => PickupBase == null ? Player.Get(ItemBase.Owner) : Player.Get(PickupBase.PreviousOwner.Hub);

        private Vector3 position = Vector3.zero;
        public Vector3 Position
        {
            get
            {
                if (ItemBase != null) return Holder.Position;
                if (PickupBase != null) return PickupBase.Info.Position;
                return position;
            }
            set
            {
                if (PickupBase != null)
                {
                    PickupBase.Rb.position = position;
                    PickupBase.RefreshPositionAndRotation();
                }
                position = value;
            }
        }
        private Quaternion rotation = default;
        public Quaternion Rotation
        {
            get
            {
                if (ItemBase != null) return Holder.FullRotation;
                if (PickupBase != null) return PickupBase.transform.rotation;
                return rotation;
            }
            set
            {
                if (PickupBase != null)
                {
                    PickupBase.transform.rotation = rotation;
                    PickupBase.RefreshPositionAndRotation();
                }
                rotation = value;
            }
        }
        private Vector3 scale = Vector3.one;
        public Vector3 Scale
        {
            get => scale;
            set
            {
                if (PickupBase != null)
                {
                    NetworkServer.UnSpawn(PickupBase.gameObject);
                    PickupBase.transform.localScale = value;
                    NetworkServer.Spawn(PickupBase.gameObject);
                }
                scale = value;
            }
        }
        public float Durabillity
        {
            get
            {
                switch (Category)
                {
                    case ItemCategory.Radio:
                        if (State == ItemState.Inventory) return (ItemBase as RadioItem).BatteryPercent;
                        else if (State == ItemState.Map) return (PickupBase as RadioPickup).SavedBattery * 100;
                        break;
                    case ItemCategory.MicroHID:
                        if (State == ItemState.Inventory) return (ItemBase as MicroHIDItem).RemainingEnergy * 100;
                        else if (State == ItemState.Map) return (PickupBase as MicroHIDPickup).Energy * 100;
                        break;
                    case ItemCategory.Firearm:
                        if (State == ItemState.Inventory) return (ItemBase as Firearm).Status.Ammo;
                        else if (State == ItemState.Map) return (PickupBase as FirearmPickup).Status.Ammo;
                        break;
                    case ItemCategory.Ammo:
                        if (State == ItemState.Map) return (PickupBase as AmmoPickup).SavedAmmo;
                        break;
                }
                return 0;
            }
            set
            {
                switch (Category)
                {
                    case ItemCategory.Radio:
                        if (State == ItemState.Inventory) (ItemBase as RadioItem)._battery = value / 100;
                        else if (State == ItemState.Map) (PickupBase as RadioPickup).SavedBattery = value / 100;
                        break;
                    case ItemCategory.MicroHID:
                        if (State == ItemState.Inventory) (ItemBase as MicroHIDItem).RemainingEnergy = value / 100;
                        else if (State == ItemState.Map) (PickupBase as MicroHIDPickup).Energy = value / 100;
                        break;
                    case ItemCategory.Firearm:
                        if (State == ItemState.Inventory)
                        {
                            var arm = ItemBase as Firearm;
                            arm.Status = new FirearmStatus((byte)value, arm.Status.Flags, arm.Status.Attachments);
                        }
                        else if (State == ItemState.Map)
                        {
                            var armpickup = PickupBase as FirearmPickup;
                            armpickup.Status = new FirearmStatus((byte)value, armpickup.Status.Flags, armpickup.Status.Attachments);
                        }
                        break;

                    case ItemCategory.Ammo:
                        if (State == ItemState.Map) (PickupBase as AmmoPickup).SavedAmmo = (ushort)value;
                        break;
                }
            }
        }
        public uint WeaponAttachments
        {
            get
            {
                if (ItemBase is Firearm arm)
                {
                    return arm.Status.Attachments;
                }
                else if (PickupBase is FirearmPickup armpickup)
                {
                    return armpickup.Status.Attachments;
                }
                return 0;
            }
            set
            {
                if (ItemBase is Firearm arm)
                {
                    var newstatus = new FirearmStatus(arm.Status.Ammo, arm.Status.Flags, value);
                    arm.OnStatusChanged(arm.Status, newstatus);
                }
                else if (PickupBase is FirearmPickup armpickup)
                {
                    armpickup.NetworkStatus = new FirearmStatus(armpickup.Status.Ammo, armpickup.Status.Flags, value);
                }
            }
        }

        public void PickUp(Player player)
        {
            switch (State)
            {
                case ItemState.Map:
                    player.DefaultInventory.ServerAddItem(Type, Serial, PickupBase);
                    break;
                case ItemState.Despawned:
                    player.DefaultInventory.ServerAddItem(Type, Serial);
                    break;
                case ItemState.Inventory:
                    Despawn();
                    player.DefaultInventory.ServerAddItem(Type, Serial);
                    break;
            }
        }
        public void Drop(Vector3 position)
        {
            switch (State)
            {
                case ItemState.Map: Position = position; break;

                case ItemState.Inventory:
                    Despawn();
                    goto case ItemState.Despawned;

                case ItemState.Despawned:
                    if (InventoryItemLoader.AvailableItems.TryGetValue(Type, out var examplebase))
                        ReferenceHub.LocalHub.inventory.ServerCreatePickup(examplebase, new PickupSyncInfo
                        {
                            ItemId = Type,
                            Serial = Serial,
                            Weight = Weight
                        }, true);

                    Position = position;
                    break;
            }
        }
        public void Drop()
        {
            switch (State)
            {
                case ItemState.Inventory: Holder.DefaultInventory.ServerDropItem(Serial); break;
                case ItemState.Despawned: Drop(Position); break;
            }
        }
        public void Despawn()
        {
            if (ItemBase != null) ItemBase.OwnerInventory.ServerRemoveItem(Serial, null);
            if (PickupBase != null) NetworkServer.Destroy(PickupBase.gameObject);
        }
        public void Destroy()
        {
            Despawn();
            AllItems.Remove(Serial);
            deactivated = true;
        }
        public class Information
        {
            public int ID;
            public ItemType BasedItemType;
            public string Name;
        }
        public static class Manager
        {
            public const int Highest = (int)ItemType.GunShotgun;
            private static readonly List<Information> custom = new List<Information>();
            public static ItemType GetBaseType(int id)
            {
                if (id >= 0 && id <= 35) return (ItemType)id;
                if (!IsIDRegistered(id)) return ItemType.None;
                var item = custom.FirstOrDefault(x => x.ID == id);
                return item.BasedItemType;
            }
            public static string GetName(int id)
            {
                if (id >= 0 && id <= Highest) return ((ItemType)id).ToString();
                if (!IsIDRegistered(id)) return "";
                var item = custom.FirstOrDefault(x => x.ID == id);
                return item.Name;
            }
            public static Information GetInfo(int id) => custom.FirstOrDefault(x => x.ID == id);
            public static void RegisterCustomItem(Information info)
            {
                if (info.ID >= 0 && info.ID <= Highest) return;
                if (custom.Select(x => x.ID).Contains(info.ID)) return;
                custom.Add(info);
            }
            public static bool IsIDRegistered(int id)
            {
                if (id >= 0 && id <= Highest) return true;
                if (custom.Any(x => x.ID == id)) return true;
                return false;
            }
        }
    }
}