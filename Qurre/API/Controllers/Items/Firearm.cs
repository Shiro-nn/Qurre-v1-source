﻿using CameraShaking;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.Modules;
using Qurre.API.Objects;
namespace Qurre.API.Controllers.Items
{
    public class Firearm : Item
    {
        public Firearm(InventorySystem.Items.Firearms.Firearm itemBase)
            : base(itemBase)
        {
            Base = itemBase;

            Base.AmmoManagerModule = Base switch
            {
                AutomaticFirearm auto => new AutomaticAmmoManager(auto, auto._baseMaxAmmo, 1, auto._boltTravelTime == 0),
                Shotgun shotgun => new TubularMagazineAmmoManager(shotgun, Serial, shotgun._ammoCapacity, shotgun._numberOfChambers, 0.5f, 3, "ShellsToLoad", ActionName.Zoom, ActionName.Shoot),
                _ => new ClipLoadedInternalMagAmmoManager(Base, 6),
            };
            Base._status = new FirearmStatus(MaxAmmo, FirearmStatusFlags.MagazineInserted, 0);
        }
        public Firearm(ItemType type)
            : this((InventorySystem.Items.Firearms.Firearm)Server.Host.Inventory.CreateItemInstance(type, false))
        {
        }
        public new InventorySystem.Items.Firearms.Firearm Base { get; }
        public byte Ammo
        {
            get => Base.Status.Ammo;
            set
            {
                FirearmStatus status = Base._status;
                Base._status = new FirearmStatus(value, Base.Status.Flags, Base.Status.Attachments);
                Base.OnStatusChanged(status, Base._status);
            }
        }
        public byte MaxAmmo => Base.AmmoManagerModule.MaxAmmo;
        public AmmoType AmmoType => Base.AmmoType.GetAmmoType();
        public DamageTypes.DamageType DamageType => Base.DamageType;
        public FirearmAttachment[] Attachments
        {
            get => Base.Attachments;
            set => Base.Attachments = value;
        }
        public float FireRate
        {
            get => Base is AutomaticFirearm auto ? auto._fireRate : 1f;
            set
            {
                if (Base is AutomaticFirearm auto)
                    auto._fireRate = value;
                else Log.Warn("You cannot change the firerate of non-automatic weapons.");
            }
        }
        public RecoilSettings Recoil
        {
            get => Base is AutomaticFirearm auto ? auto._recoil : default;
            set
            {
                if (Base is AutomaticFirearm auto)
                    auto.ActionModule = new AutomaticAction(Base, auto._semiAutomatic, auto._boltTravelTime, 1f / auto._fireRate, auto._dryfireClipId, auto._triggerClipId, auto._gunshotPitchRandomization, value, auto._recoilPattern);
                else Log.Warn("You cannot change the recoil pattern of non-automatic weapons.");
            }
        }
    }
}