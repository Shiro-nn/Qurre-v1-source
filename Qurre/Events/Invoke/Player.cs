using Qurre.API.Events;
using static Qurre.Events.Player;
namespace Qurre.Events.Invoke
{
    public static class Player
    {
        public static void Banned(BannedEvent ev) => Invokes(ev);
        public static void Ban(BanEvent ev) => Invokes(ev);
        public static void Kick(KickEvent ev) => Invokes(ev);
        public static void Kicked(KickedEvent ev) => Invokes(ev);
        public static void GroupChange(GroupChangeEvent ev) => Invokes(ev);
        public static void ItemChange(ItemChangeEvent ev) => Invokes(ev);
        public static void RoleChange(RoleChangeEvent ev) => Invokes(ev);
        public static void Dead(DeadEvent ev) => Invokes(ev);
        public static void Escape(EscapeEvent ev) => Invokes(ev);
        public static void Cuff(CuffEvent ev) => Invokes(ev);
        public static void UnCuff(UnCuffEvent ev) => Invokes(ev);
        public static void Damage(DamageEvent ev) => Invokes(ev);
        public static void Dies(DiesEvent ev) => Invokes(ev);
        public static void Interact(InteractEvent ev) => Invokes(ev);
        public static void InteractDoor(InteractDoorEvent ev) => Invokes(ev);
        public static void InteractLift(InteractLiftEvent ev) => Invokes(ev);
        public static void InteractLocker(InteractLockerEvent ev) => Invokes(ev);
        public static void IcomSpeak(IcomSpeakEvent ev) => Invokes(ev);
        public static void DroppingItem(DroppingItemEvent ev) => Invokes(ev);
        public static void DropItem(DropItemEvent ev) => Invokes(ev);
        public static void Join(JoinEvent ev) => Invokes(ev);
        public static void Leave(LeaveEvent ev) => Invokes(ev);
        public static void PickupItem(PickupItemEvent ev) => Invokes(ev);
        public static void RechargeWeapon(RechargeWeaponEvent ev) => Invokes(ev);
        public static void Shooting(ShootingEvent ev) => Invokes(ev);
        public static void RagdollSpawn(RagdollSpawnEvent ev) => Invokes(ev);
        public static void Heal(HealEvent ev) => Invokes(ev);
        public static void MedicalStopping(MedicalStoppingEvent ev) => Invokes(ev);
        public static void MedicalUsing(MedicalUsingEvent ev) => Invokes(ev);
        public static void MedicalUsed(MedicalUsedEvent ev) => Invokes(ev);
        public static void SyncData(SyncDataEvent ev) => Invokes(ev);
        public static void ThrowGrenade(ThrowGrenadeEvent ev) => Invokes(ev);
        public static void TeslaTrigger(TeslaTriggerEvent ev) => Invokes(ev);
        public static void InteractGenerator(InteractGeneratorEvent ev) => Invokes(ev);
        public static void Speak(SpeakEvent ev) => Invokes(ev);
        public static void Spawn(SpawnEvent ev) => Invokes(ev);
        public static void RadioUpdate(RadioUpdateEvent ev) => Invokes(ev);
        public static void TransmitPlayerData(TransmitPlayerDataEvent ev) => Invokes(ev);
    }
}