﻿using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class Player
    {
        public static event AllEvents<BannedEvent> Banned;
        public static event AllEvents<BanEvent> Ban;
        public static event AllEvents<KickEvent> Kick;
        public static event AllEvents<GroupChangeEvent> GroupChange;
        public static event AllEvents<ItemChangeEvent> ItemChange;
        public static event AllEvents<RoleChangeEvent> RoleChange;
        public static event AllEvents<DiedEvent> Died;
        public static event AllEvents<EscapeEvent> Escape;
        public static event AllEvents<CuffEvent> Cuff;
        public static event AllEvents<UnCuffEvent> UnCuff;
        public static event AllEvents<DamageEvent> Damage;
        public static event AllEvents<DyingEvent> Dying;
        public static event AllEvents<InteractEvent> Interact;
        public static event AllEvents<InteractDoorEvent> InteractDoor;
        public static event AllEvents<InteractLiftEvent> InteractLift;
        public static event AllEvents<InteractLockerEvent> InteractLocker;
        public static event AllEvents<IcomSpeakEvent> IcomSpeak;
        public static event AllEvents<DroppingItemEvent> DroppingItem;
        public static event AllEvents<DropItemEvent> DropItem;
        public static event AllEvents<JoinEvent> Join;
        public static event AllEvents<KickedEvent> Kicked;
        public static event AllEvents<LeaveEvent> Leave;
        public static event AllEvents<PickupItemEvent> PickupItem;
        public static event AllEvents<RechargeWeaponEvent> RechargeWeapon;
        public static event AllEvents<ShootingEvent> Shooting;
        public static event AllEvents<RagdollSpawnEvent> RagdollSpawn;
        public static event AllEvents<StoppingMedicalUsingEvent> StoppingMedicalUsing;
        public static event AllEvents<UsingMedicalEvent> UsingMedical;
        public static event AllEvents<UsedMedicalEvent> UsedMedical;
        public static event AllEvents<SyncDataEvent> SyncData;
        public static event AllEvents<ThrowGrenadeEvent> ThrowGrenade;
        public static event AllEvents<TeslaTriggerEvent> TeslaTrigger;
        public static event AllEvents<InteractGeneratorEvent> InteractGenerator;
        public static event AllEvents<SpeakEvent> Speak;
        public static void banned(BannedEvent ev) => Banned.invoke(ev);
        public static void ban(BanEvent ev) => Ban.invoke(ev);
        public static void kick(KickEvent ev) => Kick.invoke(ev);
        public static void groupchange(GroupChangeEvent ev) => GroupChange.invoke(ev);
        public static void itemchange(ItemChangeEvent ev) => ItemChange.invoke(ev);
        public static void rolechange(RoleChangeEvent ev) => RoleChange.invoke(ev);
        public static void died(DiedEvent ev) => Died.invoke(ev);
        public static void escape(EscapeEvent ev) => Escape.invoke(ev);
        public static void cuff(CuffEvent ev) => Cuff.invoke(ev);
        public static void unCuff(UnCuffEvent ev) => UnCuff.invoke(ev);
        public static void damage(DamageEvent ev) => Damage.invoke(ev);
        public static void dying(DyingEvent ev) => Dying.invoke(ev);
        public static void interact(InteractEvent ev) => Interact.invoke(ev);
        public static void interactdoor(InteractDoorEvent ev) => InteractDoor.invoke(ev);
        public static void interactLift(InteractLiftEvent ev) => InteractLift.invoke(ev);
        public static void interactLocker(InteractLockerEvent ev) => InteractLocker.invoke(ev);
        public static void icomSpeak(IcomSpeakEvent ev) => IcomSpeak.invoke(ev);
        public static void droppingItem(DroppingItemEvent ev) => DroppingItem.invoke(ev);
        public static void dropItem(DropItemEvent ev) => DropItem.invoke(ev);
        public static void join(JoinEvent ev) => Join.invoke(ev);
        public static void kicked(KickedEvent ev) => Kicked.invoke(ev);
        public static void leave(LeaveEvent ev) => Leave.invoke(ev);
        public static void pickupItem(PickupItemEvent ev) => PickupItem.invoke(ev);
        public static void rechargeWeapon(RechargeWeaponEvent ev) => RechargeWeapon.invoke(ev);
        public static void shooting(ShootingEvent ev) => Shooting.invoke(ev);
        public static void ragdollSpawn(RagdollSpawnEvent ev) => RagdollSpawn.invoke(ev);
        public static void stoppingMedicalUsing(StoppingMedicalUsingEvent ev) => StoppingMedicalUsing.invoke(ev);
        public static void usingMedical(UsingMedicalEvent ev) => UsingMedical.invoke(ev);
        public static void usedMedical(UsedMedicalEvent ev) => UsedMedical.invoke(ev);
        public static void syncData(SyncDataEvent ev) => SyncData.invoke(ev);
        public static void throwGrenade(ThrowGrenadeEvent ev) => ThrowGrenade.invoke(ev);
        public static void teslaTrigger(TeslaTriggerEvent ev) => TeslaTrigger.invoke(ev);
        public static void interactGenerator(InteractGeneratorEvent ev) => InteractGenerator.invoke(ev);
        public static void speak(SpeakEvent ev) => Speak.invoke(ev);
    }
}