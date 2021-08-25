﻿using System.Collections.Generic;
using System.Linq;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using firearm = InventorySystem.Items.Firearms.Firearm;
using InventorySystem.Items.Firearms.Ammo;
using InventorySystem.Items.Flashlight;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Radio;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using Mirror;
using Qurre.API.Controllers.Items;
using UnityEngine;
using InventorySystem.Items.Firearms;
namespace Qurre.API.Controllers
{
    public class Item
    {
        public static Item None { get; } = new Item(-1);
        private Item(int id)
        {
            if (id == -1 && None == null)
            {
                Type = ItemType.None;
                Category = ItemCategory.None;
            }
        }
        internal static readonly Dictionary<ItemBase, Item> BaseToItem = new Dictionary<ItemBase, Item>();
        private ushort id;
        public Item(ItemBase itemBase)
        {
            Base = itemBase;
            Type = itemBase.ItemTypeId;
            Category = itemBase.Category;
            Serial = Base.OwnerInventory.UserInventory.Items.FirstOrDefault(i => i.Value == Base).Key;
            if (Serial == 0) Serial = ItemSerialGenerator.GenerateNext();
            BaseToItem.Add(itemBase, this);
        }
        public Item(ItemType type) : this(Server.Host.Inventory.CreateItemInstance(type, false)) { }
        public ushort Serial
        {
            get
            {
                id = Base.OwnerInventory.UserInventory.Items.FirstOrDefault(i => i.Value == Base).Key;
                return id;
            }
            internal set
            {
                if (value == 0) value = ItemSerialGenerator.GenerateNext();
                if (Base == null || Base.PickupDropModel == null)
                    return;
                Base.PickupDropModel.Info.Serial = value;
                Base.PickupDropModel.NetworkInfo = Base.PickupDropModel.Info;
                id = value;
            }
        }
        public Vector3 Scale { get; set; } = Vector3.one;
        public ItemBase Base { get; }
        public ItemType Type { get; internal set; }
        public ItemCategory Category { get; internal set; }
        public float Weight => Base.Weight;
        public Player Owner
        {
            get
            {
                if(Base != null) return Player.Get(Base.Owner);
                return Server.Host;
            }
        }
        public static Item Get(ItemBase itemBase)
        {
            if (itemBase == null)
                return null;

            if (BaseToItem.ContainsKey(itemBase))
                return BaseToItem[itemBase];

            return itemBase switch
            {
                firearm firearm => new Items.Firearm(firearm),
                KeycardItem keycard => new Keycard(keycard),
                UsableItem usable => new Usable(usable),
                RadioItem radio => new Items.Radio(radio),
                MicroHIDItem micro => new MicroHid(micro),
                BodyArmor armor => new Armor(armor),
                AmmoItem ammo => new Ammo(ammo),
                FlashlightItem flashlight => new Flashlight(flashlight),
                ThrowableItem throwable => throwable.Projectile switch
                {
                    FlashbangGrenade _ => new FlashGrenade(throwable),
                    ExplosionGrenade _ => new ExplosiveGrenade(throwable),
                    _ => new Throwable(throwable),
                },
                _ => new Item(itemBase),
            };
        }
        public static Item Get(ushort serial)
        {
            var _ = Object.FindObjectsOfType<ItemBase>().Where(x => x.ItemSerial == serial);
            if (_.Count() == 0) return null;
            return Get(_.First());
        }
        public void Give(Player player) => player.AddItem(Base);
        public virtual Pickup Spawn(Vector3 position, Quaternion rotation = default)
        {
            Base.PickupDropModel.Info.ItemId = Type;
            Base.PickupDropModel.Info.Position = position;
            Base.PickupDropModel.Info.Weight = Weight;
            Base.PickupDropModel.Info.Rotation = new LowPrecisionQuaternion(rotation);
            Base.PickupDropModel.NetworkInfo = Base.PickupDropModel.Info;

            ItemPickupBase ipb = Object.Instantiate(Base.PickupDropModel, position, rotation);
            if (ipb is FirearmPickup firearmPickup)
            {
                if (this is Items.Firearm firearm)
                {
                    firearmPickup.Status = new FirearmStatus(firearm.Ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }
                else
                {
                    byte ammo = Base switch
                    {
                        AutomaticFirearm auto => auto._baseMaxAmmo,
                        Shotgun shotgun => shotgun._ammoCapacity,
                        Revolver _ => 6,
                        _ => 0,
                    };
                    firearmPickup.Status = new FirearmStatus(ammo, FirearmStatusFlags.MagazineInserted, firearmPickup.Status.Attachments);
                }

                firearmPickup.NetworkStatus = firearmPickup.Status;
            }

            NetworkServer.Spawn(ipb.gameObject);
            ipb.InfoReceived(default, Base.PickupDropModel.NetworkInfo);
            Pickup pickup = Pickup.Get(ipb);
            pickup.Scale = Scale;
            return pickup;
        }
    }
}