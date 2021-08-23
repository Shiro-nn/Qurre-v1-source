using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Controllers.Items
{
    public class Pickup
    {
        internal static readonly Dictionary<ItemPickupBase, Pickup> BaseToItem = new Dictionary<ItemPickupBase, Pickup>();
        private ushort id;
        public Pickup(ItemPickupBase pickupBase)
        {
            Base = pickupBase;
            Serial = pickupBase.NetworkInfo.Serial;
            BaseToItem.Add(pickupBase, this);
        }
        public Pickup(ItemType type)
        {
            if (!InventoryItemLoader.AvailableItems.TryGetValue(type, out ItemBase itemBase))
                return;

            Base = itemBase.PickupDropModel;
            Serial = itemBase.PickupDropModel.NetworkInfo.Serial;
            BaseToItem.Add(itemBase.PickupDropModel, this);
        }
        public ushort Serial
        {
            get
            {
                if (id == 0)
                {
                    id = ItemSerialGenerator.GenerateNext();
                    Base.Info.Serial = id;
                    Base.NetworkInfo = Base.Info;
                }

                return id;
            }

            internal set => id = value;
        }
        public Vector3 Scale
        {
            get => Base.gameObject.transform.localScale;
            set
            {
                GameObject gameObject = Base.gameObject;
                NetworkServer.UnSpawn(gameObject);
                gameObject.transform.localScale = value;
                NetworkServer.Spawn(gameObject);
                gameObject.transform.localScale = Vector3.one;
            }
        }
        public float Weight
        {
            get => Base.NetworkInfo.Weight;
            set
            {
                Base.Info.Weight = value;
                Base.NetworkInfo = Base.Info;
            }
        }
        public ItemPickupBase Base { get; }
        public ItemType Type => Base.NetworkInfo.ItemId;
        public bool Locked
        {
            get => Base.NetworkInfo.Locked;
            set
            {
                Base.Info.Locked = value;
                Base.NetworkInfo = Base.Info;
            }
        }
        public bool InUse
        {
            get => Base.NetworkInfo.InUse;
            set
            {
                Base.Info.InUse = value;
                Base.NetworkInfo = Base.Info;
            }
        }
        public Vector3 Position
        {
            get => Base.NetworkInfo.Position;
            set
            {
                Base.Rb.position = value;
                Base.transform.position = value;
                NetworkServer.UnSpawn(Base.gameObject);
                NetworkServer.Spawn(Base.gameObject);

                Base.RefreshPositionAndRotation();
            }
        }
        public Quaternion Rotation
        {
            get => Base.NetworkInfo.Rotation.Value;
            set
            {
                Base.Rb.rotation = value;
                Base.transform.rotation = value;
                NetworkServer.UnSpawn(Base.gameObject);
                NetworkServer.Spawn(Base.gameObject);

                Base.RefreshPositionAndRotation();
            }
        }
        public static Pickup Get(ItemPickupBase pickupBase) =>
            pickupBase == null ? null :
            BaseToItem.ContainsKey(pickupBase) ? BaseToItem[pickupBase] :
            new Pickup(pickupBase);
        public void Destroy()
        {
            Base.DestroySelf();
            BaseToItem.Remove(Base);
        }
    }
}