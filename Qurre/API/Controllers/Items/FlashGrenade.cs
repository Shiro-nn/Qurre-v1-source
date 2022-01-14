using InventorySystem.Items.ThrowableProjectiles;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Controllers.Items
{
    public class FlashGrenade : Throwable
    {
        internal static Dictionary<FlashbangGrenade, FlashGrenade> GTB { get; set; } = new();
        public FlashGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }
        public FlashGrenade(ItemType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true))
        {
        }
        public new FlashbangGrenade Projectile => (FlashbangGrenade)Base.Projectile;
        public AnimationCurve BlindCurve
        {
            get => Projectile._blindingOverDistance;
            set => Projectile._blindingOverDistance = value;
        }
        public float SurfaceDistanceIntensifier
        {
            get => Projectile._surfaceZoneDistanceIntensifier;
            set => Projectile._surfaceZoneDistanceIntensifier = value;
        }
        public AnimationCurve DeafenCurve
        {
            get => Projectile._deafenDurationOverDistance;
            set => Projectile._deafenDurationOverDistance = value;
        }
        public float FuseTime
        {
            get => Projectile._fuseTime;
            set => Projectile._fuseTime = value;
        }
    }
}