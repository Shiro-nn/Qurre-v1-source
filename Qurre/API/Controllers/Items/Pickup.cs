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
        private ItemCategory category = ItemCategory.None;
        public ItemCategory Category
        {
            get
            {
                if (category == ItemCategory.None) category = Get();
                return category;
                ItemCategory Get()
                {
                    switch (Type)
                    {
                        case ItemType.KeycardChaosInsurgency:
                        case ItemType.KeycardContainmentEngineer:
                        case ItemType.KeycardFacilityManager:
                        case ItemType.KeycardGuard:
                        case ItemType.KeycardJanitor:
                        case ItemType.KeycardNTFCommander:
                        case ItemType.KeycardNTFLieutenant:
                        case ItemType.KeycardNTFOfficer:
                        case ItemType.KeycardO5:
                        case ItemType.KeycardResearchCoordinator:
                        case ItemType.KeycardScientist:
                        case ItemType.KeycardZoneManager:
                            return ItemCategory.Keycard;
                        case ItemType.Medkit:
                        case ItemType.Adrenaline:
                        case ItemType.Painkillers:
                            return ItemCategory.Medical;
                        case ItemType.Radio:
                            return ItemCategory.Radio;
                        case ItemType.GunAK:
                        case ItemType.GunCOM15:
                        case ItemType.GunCOM18:
                        case ItemType.GunCrossvec:
                        case ItemType.GunE11SR:
                        case ItemType.GunFSP9:
                        case ItemType.GunLogicer:
                        case ItemType.GunRevolver:
                        case ItemType.GunShotgun:
                            return ItemCategory.Firearm;
                        case ItemType.GrenadeFlash:
                        case ItemType.GrenadeHE:
                            return ItemCategory.Grenade;
                        case ItemType.SCP018:
                        case ItemType.SCP207:
                        case ItemType.SCP268:
                        case ItemType.SCP500:
                            return ItemCategory.SCPItem;
                        case ItemType.MicroHID:
                            return ItemCategory.MicroHID;
                        case ItemType.Ammo12gauge:
                        case ItemType.Ammo44cal:
                        case ItemType.Ammo556x45:
                        case ItemType.Ammo762x39:
                        case ItemType.Ammo9x19:
                            return ItemCategory.Ammo;
                        case ItemType.ArmorCombat:
                        case ItemType.ArmorHeavy:
                        case ItemType.ArmorLight:
                            return ItemCategory.Armor;
                        default:
                            return ItemCategory.None;
                    }
                }
            }
        }
        public bool Locked
        {
            get => Base.NetworkInfo.Locked;
            set
            {
                PickupSyncInfo info = Base.Info;
                info.Locked = value;
                Base.NetworkInfo = info;
            }
        }
        public bool InUse
        {
            get => Base.NetworkInfo.InUse;
            set
            {
                PickupSyncInfo info = Base.Info;
                info.InUse = value;
                Base.NetworkInfo = info;
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