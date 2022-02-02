using InventorySystem.Items.ThrowableProjectiles;
namespace Qurre.API.Controllers.Items
{
    [System.Obsolete("Use 'GrenadeFrag'")]
    public class ExplosiveGrenade : Throwable
    {
        public ExplosiveGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
            ExplosionGrenade grenade = (ExplosionGrenade)Base.Projectile;
            MaxRadius = grenade._maxRadius;
            ScpMultiplier = grenade._scpDamageMultiplier;
            BurnDuration = grenade._burnedDuration;
            DeafenDuration = grenade._deafenedDuration;
            ConcussDuration = grenade._concussedDuration;
            FuseTime = grenade._fuseTime;
        }
        public ExplosiveGrenade(ItemType type, Player player = null)
            : this(player == null ? (ThrowableItem)Server.Host.Inventory.CreateItemInstance(type, false) : (ThrowableItem)player.Inventory.CreateItemInstance(type, true))
        {
        }
        public new ExplosionGrenade Projectile => (ExplosionGrenade)Base.Projectile;
        public float MaxRadius
        {
            get => Projectile._maxRadius;
            set => Projectile._maxRadius = value;
        }
        public float ScpMultiplier
        {
            get => Projectile._scpDamageMultiplier;
            set => Projectile._scpDamageMultiplier = value;
        }
        public float BurnDuration
        {
            get => Projectile._burnedDuration;
            set => Projectile._burnedDuration = value;
        }
        public float DeafenDuration
        {
            get => Projectile._deafenedDuration;
            set => Projectile._deafenedDuration = value;
        }
        public float ConcussDuration
        {
            get => Projectile._concussedDuration;
            set => Projectile._concussedDuration = value;
        }
        public float FuseTime
        {
            get => Projectile._fuseTime;
            set => Projectile._fuseTime = value;
        }
    }
}