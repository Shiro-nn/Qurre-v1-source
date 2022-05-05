﻿using Qurre.API.Events;
using Qurre.Events.Modules;
using static Qurre.Events.Modules.Main;

namespace Qurre.Events
{
    public static class Player
    {
        public static event AllEvents<BannedEvent> Banned;

        public static event AllEvents<BanEvent> Ban;

        public static event AllEvents<KickEvent> Kick;

        public static event AllEvents<KickedEvent> Kicked;

        public static event AllEvents<GroupChangeEvent> GroupChange;

        public static event AllEvents<ItemChangeEvent> ItemChange;

        public static event AllEvents<RoleChangeEvent> RoleChange;

        public static event AllEvents<DeadEvent> Dead;

        public static event AllEvents<EscapeEvent> Escape;

        public static event AllEvents<CuffEvent> Cuff;

        public static event AllEvents<UnCuffEvent> UnCuff;

        public static event AllEvents<DamageEvent> Damage;

        public static event AllEvents<DamageProcessEvent> DamageProcess;

        public static event AllEvents<DiesEvent> Dies;

        public static event AllEvents<InteractEvent> Interact;

        public static event AllEvents<InteractDoorEvent> InteractDoor;

        public static event AllEvents<InteractLiftEvent> InteractLift;

        public static event AllEvents<InteractLockerEvent> InteractLocker;

        public static event AllEvents<InteractScp330Event> InteractScp330;

        public static event AllEvents<EatingScp330Event> EatingScp330;

        public static event AllEvents<PickupCandyEvent> PickupCandy;

        public static event AllEvents<IcomSpeakEvent> IcomSpeak;

        public static event AllEvents<DroppingItemEvent> DroppingItem;

        public static event AllEvents<DropItemEvent> DropItem;

        public static event AllEvents<JoinEvent> Join;

        public static event AllEvents<LeaveEvent> Leave;

        public static event AllEvents<PickupItemEvent> PickupItem;

        public static event AllEvents<RechargeWeaponEvent> RechargeWeapon;

        public static event AllEvents<ShootingEvent> Shooting;

        public static event AllEvents<RagdollSpawnEvent> RagdollSpawn;

        public static event AllEvents<HealEvent> Heal;

        public static event AllEvents<ItemStoppingEvent> ItemStopping;

        public static event AllEvents<ItemUsingEvent> ItemUsing;

        public static event AllEvents<ItemUsedEvent> ItemUsed;

        public static event AllEvents<SyncDataEvent> SyncData;

        public static event AllEvents<ThrowItemEvent> ThrowItem;

        public static event AllEvents<TeslaTriggerEvent> TeslaTrigger;

        public static event AllEvents<InteractGeneratorEvent> InteractGenerator;

        public static event AllEvents<SpawnEvent> Spawn;

        public static event AllEvents<RadioUpdateEvent> RadioUpdate;

        public static event AllEvents<TransmitPlayerDataEvent> TransmitPlayerData;

        public static event AllEvents<MicroHidUsingEvent> MicroHidUsing;

        public static event AllEvents<RadioUsingEvent> RadioUsing;

        public static event AllEvents<FlashExplosionEvent> FlashExplosion;

        public static event AllEvents<FragExplosionEvent> FragExplosion;

        public static event AllEvents<FlashedEvent> Flashed;

        public static event AllEvents<DropAmmoEvent> DropAmmo;

        public static event AllEvents<ScpAttackEvent> ScpAttack;

        public static event AllEvents<SinkholeWalkingEvent> SinkholeWalking;

        public static event AllEvents<TantrumWalkingEvent> TantrumWalking;

        public static event AllEvents<ChangeSpectateEvent> ChangeSpectate;

        public static event AllEvents<ZoomingEvent> Zooming;

        public static event AllEvents<CoinFlipEvent> CoinFlip;

        public static event AllEvents<HideBadgeEvent> HideBadge;

        public static event AllEvents<ShowBadgeEvent> ShowBadge;

        public static event AllEvents<JumpEvent> Jump;

        public static event AllEvents<HotKeyPressEvent> HotKeyPress;

        internal static void Invokes(BannedEvent ev) => Banned.CustomInvoke(ev);

        internal static void Invokes(BanEvent ev) => Ban.CustomInvoke(ev);

        internal static void Invokes(KickEvent ev) => Kick.CustomInvoke(ev);

        internal static void Invokes(KickedEvent ev) => Kicked.CustomInvoke(ev);

        internal static void Invokes(GroupChangeEvent ev) => GroupChange.CustomInvoke(ev);

        internal static void Invokes(ItemChangeEvent ev) => ItemChange.CustomInvoke(ev);

        internal static void Invokes(RoleChangeEvent ev) => RoleChange.CustomInvoke(ev);

        internal static void Invokes(DeadEvent ev) => Dead.CustomInvoke(ev);

        internal static void Invokes(EscapeEvent ev) => Escape.CustomInvoke(ev);

        internal static void Invokes(CuffEvent ev) => Cuff.CustomInvoke(ev);

        internal static void Invokes(UnCuffEvent ev) => UnCuff.CustomInvoke(ev);

        internal static void Invokes(DamageEvent ev) => Damage.CustomInvoke(ev);

        internal static void Invokes(DamageProcessEvent ev) => DamageProcess.CustomInvoke(ev);

        internal static void Invokes(DiesEvent ev) => Dies.CustomInvoke(ev);

        internal static void Invokes(InteractEvent ev) => Interact.CustomInvoke(ev);

        internal static void Invokes(InteractDoorEvent ev) => InteractDoor.CustomInvoke(ev);

        internal static void Invokes(InteractLiftEvent ev) => InteractLift.CustomInvoke(ev);

        internal static void Invokes(InteractLockerEvent ev) => InteractLocker?.CustomInvoke(ev);

        internal static void Invokes(InteractScp330Event ev) => InteractScp330.CustomInvoke(ev);

        internal static void Invokes(EatingScp330Event ev) => EatingScp330.CustomInvoke(ev);

        internal static void Invokes(PickupCandyEvent ev) => PickupCandy.CustomInvoke(ev);

        internal static void Invokes(IcomSpeakEvent ev) => IcomSpeak.CustomInvoke(ev);

        internal static void Invokes(DroppingItemEvent ev) => DroppingItem.CustomInvoke(ev);

        internal static void Invokes(DropItemEvent ev) => DropItem.CustomInvoke(ev);

        internal static void Invokes(JoinEvent ev) => Join.CustomInvoke(ev);

        internal static void Invokes(LeaveEvent ev) => Leave.CustomInvoke(ev);

        internal static void Invokes(PickupItemEvent ev) => PickupItem.CustomInvoke(ev);

        internal static void Invokes(RechargeWeaponEvent ev) => RechargeWeapon.CustomInvoke(ev);

        internal static void Invokes(ShootingEvent ev) => Shooting.CustomInvoke(ev);

        internal static void Invokes(RagdollSpawnEvent ev) => RagdollSpawn.CustomInvoke(ev);

        internal static void Invokes(HealEvent ev) => Heal.CustomInvoke(ev);

        internal static void Invokes(ItemStoppingEvent ev) => ItemStopping.CustomInvoke(ev);

        internal static void Invokes(ItemUsingEvent ev) => ItemUsing.CustomInvoke(ev);

        internal static void Invokes(ItemUsedEvent ev) => ItemUsed.CustomInvoke(ev);

        internal static void Invokes(SyncDataEvent ev) => SyncData.CustomInvoke(ev);

        internal static void Invokes(ThrowItemEvent ev) => ThrowItem.CustomInvoke(ev);

        internal static void Invokes(TeslaTriggerEvent ev) => TeslaTrigger.CustomInvoke(ev);

        internal static void Invokes(InteractGeneratorEvent ev) => InteractGenerator.CustomInvoke(ev);

        internal static void Invokes(SpawnEvent ev) => Spawn.CustomInvoke(ev);

        internal static void Invokes(RadioUpdateEvent ev) => RadioUpdate.CustomInvoke(ev);

        internal static void Invokes(TransmitPlayerDataEvent ev) => TransmitPlayerData.CustomInvoke(ev);

        internal static void Invokes(MicroHidUsingEvent ev) => MicroHidUsing.CustomInvoke(ev);

        internal static void Invokes(RadioUsingEvent ev) => RadioUsing.CustomInvoke(ev);

        internal static void Invokes(FlashExplosionEvent ev) => FlashExplosion.CustomInvoke(ev);

        internal static void Invokes(FragExplosionEvent ev) => FragExplosion.CustomInvoke(ev);

        internal static void Invokes(FlashedEvent ev) => Flashed.CustomInvoke(ev);

        internal static void Invokes(DropAmmoEvent ev) => DropAmmo.CustomInvoke(ev);

        internal static void Invokes(ScpAttackEvent ev) => ScpAttack.CustomInvoke(ev);

        internal static void Invokes(SinkholeWalkingEvent ev) => SinkholeWalking.CustomInvoke(ev);

        internal static void Invokes(TantrumWalkingEvent ev) => TantrumWalking.CustomInvoke(ev);

        internal static void Invokes(ChangeSpectateEvent ev) => ChangeSpectate.CustomInvoke(ev);

        internal static void Invokes(ZoomingEvent ev) => Zooming.CustomInvoke(ev);

        internal static void Invokes(CoinFlipEvent ev) => CoinFlip.CustomInvoke(ev);

        internal static void Invokes(ShowBadgeEvent ev) => ShowBadge.CustomInvoke(ev);

        internal static void Invokes(HideBadgeEvent ev) => HideBadge.CustomInvoke(ev);

        internal static void Invokes(JumpEvent ev) => Jump.CustomInvoke(ev);

        internal static void Invokes(HotKeyPressEvent ev) => HotKeyPress.CustomInvoke(ev);
    }
}