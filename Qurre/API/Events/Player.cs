﻿using Assets._Scripts.Dissonance;
using Grenades;
using Interactables.Interobjects.DoorUtils;
using Qurre.API.Objects;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Events
{
    public class BannedEvent : EventArgs
    {
        public BannedEvent(Player player, BanDetails details, BanHandler.BanType type)
        {
            Player = player;
            Details = details;
            Type = type;
        }
        public Player Player { get; }
        public BanDetails Details { get; }
        public BanHandler.BanType Type { get; }
    }
    public class BanEvent : KickEvent
    {
        private int duration;
        public BanEvent(Player target, Player issuer, int duration, string reason, string fullMessage, bool isAllowed = true)
            : base(target, issuer, reason, fullMessage, isAllowed)
        {
            Duration = duration;
        }
        public int Duration
        {
            get => duration;
            set
            {
                if (duration == value)
                    return;
                duration = value;
            }
        }
    }
    public class KickEvent : EventArgs
    {
        private Player target;
        private Player issuer;
        private bool isAllowed;
        public KickEvent(Player target, Player issuer, string reason, string fullMessage, bool isAllowed = true)
        {
            Target = target;
            Issuer = issuer;
            Reason = reason;
            FullMessage = fullMessage;
            IsAllowed = isAllowed;
        }
        public Player Target
        {
            get => target;
            set
            {
                if (value == null || target == value)
                    return;
                target = value;
            }
        }
        public Player Issuer
        {
            get => issuer;
            set
            {
                if (value == null || issuer == value)
                    return;
                issuer = value;
            }
        }
        public string Reason { get; set; }
        public string FullMessage { get; set; }
        public bool IsAllowed
        {
            get => isAllowed;
            set
            {
                if (isAllowed == value)
                    return;
                isAllowed = value;
            }
        }
    }
    public class KickedEvent : EventArgs
    {
        public KickedEvent(Player player, string reason, bool isAllowed = true)
        {
            Player = player;
            Reason = reason;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public string Reason { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class GroupChangeEvent : EventArgs
    {
        public GroupChangeEvent(Player player, UserGroup newGroup, bool isAllowed = true)
        {
            Player = player;
            NewGroup = newGroup;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public UserGroup NewGroup { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class ItemChangeEvent : System.EventArgs
    {
        public ItemChangeEvent(Player player, Inventory.SyncItemInfo oldItem, Inventory.SyncItemInfo newItem)
        {
            Player = player;
            OldItem = oldItem;
            NewItem = newItem;
        }
        public Player Player { get; }
        public Inventory.SyncItemInfo OldItem { get; set; }
        public Inventory.SyncItemInfo NewItem { get; }
    }
    public class RoleChangeEvent : EventArgs
    {
        public RoleChangeEvent(Player player, RoleType newRole, List<ItemType> items, bool isSavePos, bool isEscaped)
        {
            Player = player;
            NewRole = newRole;
            Items = items;
            IsSavePos = isSavePos;
            IsEscaped = isEscaped;
        }
        public Player Player { get; }
        public RoleType NewRole { get; set; }
        public List<ItemType> Items { get; }
        public bool IsEscaped { get; set; }
        public bool IsSavePos { get; set; }
    }
    public class DiedEvent : EventArgs
    {
        public DiedEvent(Player killer, Player target, PlayerStats.HitInfo hitInfo)
        {
            Killer = killer;
            Target = target;
            HitInfo = hitInfo;
        }
        public Player Killer { get; }
        public Player Target { get; }
        public PlayerStats.HitInfo HitInfo { get; set; }
    }
    public class EscapeEvent : EventArgs
    {
        public EscapeEvent(Player player, RoleType newRole, bool isAllowed = true)
        {
            Player = player;
            NewRole = newRole;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public RoleType NewRole { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class CuffEvent : EventArgs
    {
        public CuffEvent(Player cuffer, Player target, bool isAllowed = true)
        {
            Cuffer = cuffer;
            Target = target;
            IsAllowed = isAllowed;
        }
        public Player Cuffer { get; }
        public Player Target { get; }
        public bool IsAllowed { get; set; }
    }
    public class UnCuffEvent : CuffEvent
    {
        public UnCuffEvent(Player cuffer, Player target, bool isAllowed = true) : base(cuffer, target, isAllowed) { }
    }
    public class DamageEvent : EventArgs
    {
        private PlayerStats.HitInfo hitInformations;
        private DamageTypes.DamageType damageType = DamageTypes.None;
        public DamageEvent(Player attacker, Player target, PlayerStats.HitInfo hitInformations, bool isAllowed = true)
        {
            Attacker = attacker;
            Target = target;
            HitInformations = hitInformations;
            IsAllowed = isAllowed;
        }
        public Player Attacker { get; }
        public Player Target { get; }
        public PlayerStats.HitInfo HitInformations
        {
            get => hitInformations;
            private set => hitInformations = value;
        }
        public int Time => hitInformations.Time;
        public DamageTypes.DamageType DamageType
        {
            get
            {
                if (damageType == DamageTypes.None)
                    damageType = DamageTypes.FromIndex(hitInformations.Tool);

                return damageType;
            }
        }
        public int Tool => hitInformations.Tool;
        public float Amount
        {
            get => hitInformations.Amount;
            set => hitInformations.Amount = value;
        }
        public bool IsAllowed { get; set; }
    }
    public class DyingEvent : EventArgs
    {
        public DyingEvent(Player killer, Player target, PlayerStats.HitInfo hitInfo, bool isAllowed = true)
        {
            Killer = killer;
            Target = target;
            HitInfo = hitInfo;
            IsAllowed = isAllowed;
        }
        public Player Killer { get; }
        public Player Target { get; }
        public PlayerStats.HitInfo HitInfo { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class InteractEvent : EventArgs
    {
        public InteractEvent(Player player) => Player = player;
        public Player Player { get; }
    }
    public class InteractDoorEvent : EventArgs
    {
        public InteractDoorEvent(Player player, DoorVariant door, bool isAllowed = true)
        {
            Player = player;
            Door = door;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public DoorVariant Door { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class InteractGeneratorEvent : EventArgs
    {
        public InteractGeneratorEvent(Player player, Generator079 generator, GeneratorStatus status, bool isAllowed = true)
        {
            Player = player;
            Generator = generator;
            Status = status;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public Generator079 Generator { get; }
        public GeneratorStatus Status { get; }
        public bool IsAllowed { get; set; }
    }
    public class InteractLiftEvent : EventArgs
    {
        public InteractLiftEvent(Player player, Lift.Elevator elevator, Lift lift, bool isAllowed = true)
        {
            Player = player;
            Elevator = elevator;
            Lift = lift;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public Lift.Elevator Elevator { get; }
        public Lift Lift { get; }
        public bool IsAllowed { get; set; }
    }
    public class InteractLockerEvent : EventArgs
    {
        public InteractLockerEvent(Player player, Locker locker, LockerChamber lockerChamber, byte lockerId, byte chamberId, bool isAllowed)
        {
            Player = player;
            Locker = locker;
            Chamber = lockerChamber;
            LockerId = lockerId;
            ChamberId = chamberId;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public Locker Locker { get; }
        public LockerChamber Chamber { get; }
        public byte LockerId { get; }
        public byte ChamberId { get; }
        public bool IsAllowed { get; set; }
    }
    public class IcomSpeakEvent : EventArgs
    {
        public IcomSpeakEvent(Player player, bool isAllowed = true)
        {
            Player = player;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public bool IsAllowed { get; set; }
    }
    public class DroppingItemEvent : EventArgs
    {
        public DroppingItemEvent(Player player, Inventory.SyncItemInfo item, bool isAllowed = true)
        {
            Player = player;
            Item = item;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public Inventory.SyncItemInfo Item { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class DropItemEvent : EventArgs
    {
        public DropItemEvent(Player player, Pickup pickup)
        {
            Player = player;
            Pickup = pickup;
        }
        public Player Player { get; }
        public Pickup Pickup { get; }
    }
    public class PickupItemEvent : DropItemEvent
    {
        public PickupItemEvent(Player player, Pickup pickup, bool isAllowed = true) : base(player, pickup)
        {
            IsAllowed = isAllowed;
        }
        public bool IsAllowed { get; set; }
    }
    public class JoinEvent : EventArgs
    {
        public JoinEvent(Player player) => Player = player;
        public Player Player { get; }
    }
    public class LeaveEvent : JoinEvent
    {
        public LeaveEvent(Player player) : base(player) { }
    }
    public class RechargeWeaponEvent : EventArgs
    {
        public RechargeWeaponEvent(Player player, bool isAnimationOnly, bool isAllowed = true)
        {
            Player = player;
            IsAnimationOnly = isAnimationOnly;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public bool IsAnimationOnly { get; }
        public bool IsAllowed { get; set; }
    }
    public class ShootingEvent : EventArgs
    {
        public ShootingEvent(Player shooter, GameObject target, Vector3 position, bool isAllowed = true)
        {
            Shooter = shooter;
            Target = target;
            Position = position;
            IsAllowed = isAllowed;
        }
        public Player Shooter { get; }
        public GameObject Target { get; }
        public Vector3 Position { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class SpeakEvent : EventArgs
    {
        public SpeakEvent(DissonanceUserSetup userSetup, bool isIcom, bool isRadio, bool isMimicAs939, bool isScpChat, bool isRipChat, bool isAllowed = true)
        {
            UserSetup = userSetup;
            IsIntercom = isIcom;
            IsRadio = isRadio;
            IsMimicAs939 = isMimicAs939;
            IsScpChat = isScpChat;
            IsRipChat = isRipChat;
            IsAllowed = isAllowed;
        }
        public DissonanceUserSetup UserSetup { get; }
        public bool IsIntercom { get; }
        public bool IsRadio { get; }
        public bool IsMimicAs939 { get; }
        public bool IsScpChat { get; }
        public bool IsRipChat { get; }
        public bool IsAllowed { get; set; }
    }
    public class RagdollSpawnEvent : EventArgs
    {
        private int playerId;
        public RagdollSpawnEvent(
            Player killer,
            Player owner,
            Vector3 position,
            Quaternion rotation,
            RoleType roleType,
            PlayerStats.HitInfo hitInfo,
            bool isRecallAllowed,
            string dissonanceId,
            string playerName,
            int playerId,
            bool isAllowed = true)
        {
            Killer = killer;
            Owner = owner;
            Position = position;
            Rotation = rotation;
            RoleType = roleType;
            HitInfo = hitInfo;
            IsRecallAllowed = isRecallAllowed;
            DissonanceId = dissonanceId;
            PlayerNickname = playerName;
            PlayerId = playerId;
            IsAllowed = isAllowed;
        }
        public Player Killer { get; }
        public Player Owner { get; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public RoleType RoleType { get; set; }
        public PlayerStats.HitInfo HitInfo { get; set; }
        public bool IsRecallAllowed { get; set; }
        public string DissonanceId { get; set; }
        public string PlayerNickname { get; set; }
        public int PlayerId
        {
            get => playerId;
            set
            {
                if (Player.Get(value) == null)
                    return;

                playerId = value;
            }
        }
        public bool IsAllowed { get; set; }
    }
    public class UsedMedicalEvent : EventArgs
    {
        public UsedMedicalEvent(Player player, ItemType item)
        {
            Player = player;
            Item = item;
        }
        public Player Player { get; }
        public ItemType Item { get; }
    }
    public class UsingMedicalEvent : UsedMedicalEvent
    {
        public UsingMedicalEvent(Player player, ItemType item, float cooldown, bool isAllowed = true) : base(player, item)
        {
            Cooldown = cooldown;
            IsAllowed = isAllowed;
        }
        public float Cooldown { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class StoppingMedicalUsingEvent : UsingMedicalEvent
    {
        public StoppingMedicalUsingEvent(Player player, ItemType item, float cooldown, bool isAllowed = true) : base(player, item, cooldown, isAllowed) { }
        public new float Cooldown => base.Cooldown;
    }
    public class SyncDataEvent : EventArgs
    {
        public SyncDataEvent(Player player, Vector2 speed, byte currentAnimation, bool isAllowed = true)
        {
            Player = player;
            Speed = speed;
            CurrentAnimation = currentAnimation;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public Vector2 Speed { get; }
        public byte CurrentAnimation { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class ThrowGrenadeEvent : EventArgs
    {
        public ThrowGrenadeEvent(Player player, GrenadeManager grenadeManager, int id, bool isSlow, double fuseTime, bool isAllowed = true)
        {
            Player = player;
            GrenadeManager = grenadeManager;
            Id = id;
            IsSlow = isSlow;
            FuseTime = fuseTime;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public GrenadeManager GrenadeManager { get; }
        public int Id { get; }
        public bool IsSlow { get; set; }
        public double FuseTime { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class TeslaTriggerEvent : EventArgs
    {
        public TeslaTriggerEvent(Player player, bool isInHurtingRange, bool isTriggerable = true)
        {
            Player = player;
            IsInHurtingRange = isInHurtingRange;
            IsTriggerable = isTriggerable;
        }
        public Player Player { get; }
        public bool IsInHurtingRange { get; set; }
        public bool IsTriggerable { get; set; }
    }
    public class SpawnEvent : EventArgs
    {
        public SpawnEvent(Player player, RoleType roleType, Vector3 position, float rotationY)
        {
            Player = player;
            RoleType = roleType;
            Position = position;
            RotationY = rotationY;
        }
        public Player Player { get; private set; }
        public RoleType RoleType { get; private set; }
        public Vector3 Position { get; set; }
        public float RotationY { get; set; }
    }
}