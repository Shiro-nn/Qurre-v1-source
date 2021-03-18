﻿using Assets._Scripts.Dissonance;
using Grenades;
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
        public BanEvent(Player target, Player issuer, int duration, string reason, string fullMessage, bool allowed = true)
            : base(target, issuer, reason, fullMessage, allowed)
        {
            Duration = duration;
        }
        public int Duration
        {
            get => duration;
            set
            {
                if (duration == value) return;
                duration = value;
            }
        }
    }
    public class KickEvent : EventArgs
    {
        private Player target;
        private Player issuer;
        private bool _allowed;
        public KickEvent(Player target, Player issuer, string reason, string fullMessage, bool allowed = true)
        {
            Target = target;
            Issuer = issuer;
            Reason = reason;
            FullMessage = fullMessage;
            Allowed = allowed;
        }
        public Player Target
        {
            get => target;
            set
            {
                if (value == null || target == value) return;
                target = value;
            }
        }
        public Player Issuer
        {
            get => issuer;
            set
            {
                if (value == null || issuer == value) return;
                issuer = value;
            }
        }
        public string Reason { get; set; }
        public string FullMessage { get; set; }
        public bool Allowed
        {
            get => _allowed;
            set
            {
                if (_allowed == value) return;
                _allowed = value;
            }
        }
    }
    public class KickedEvent : EventArgs
    {
        public KickedEvent(Player player, string reason, bool allowed = true)
        {
            Player = player;
            Reason = reason;
            Allowed = allowed;
        }
        public Player Player { get; }
        public string Reason { get; set; }
        public bool Allowed { get; set; }
    }
    public class GroupChangeEvent : EventArgs
    {
        public GroupChangeEvent(Player player, UserGroup newGroup, bool allowed = true)
        {
            Player = player;
            NewGroup = newGroup;
            Allowed = allowed;
        }
        public Player Player { get; }
        public UserGroup NewGroup { get; set; }
        public bool Allowed { get; set; }
    }
    public class ItemChangeEvent : EventArgs
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
        public RoleChangeEvent(Player player, RoleType newRole, List<ItemType> items, bool savePos, bool escaped)
        {
            Player = player;
            NewRole = newRole;
            Items = items;
            SavePos = savePos;
            Escaped = escaped;
        }
        public Player Player { get; }
        public RoleType NewRole { get; set; }
        public List<ItemType> Items { get; }
        public bool Escaped { get; set; }
        public bool SavePos { get; set; }
    }
    public class DeadEvent : EventArgs
    {
        public DeadEvent(Player killer, Player target, PlayerStats.HitInfo hitInfo)
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
        public EscapeEvent(Player player, RoleType newRole, bool allowed = true)
        {
            Player = player;
            NewRole = newRole;
            Allowed = allowed;
        }
        public Player Player { get; }
        public RoleType NewRole { get; set; }
        public bool Allowed { get; set; }
    }
    public class CuffEvent : EventArgs
    {
        public CuffEvent(Player cuffer, Player target, bool allowed = true)
        {
            Cuffer = cuffer;
            Target = target;
            Allowed = allowed;
        }
        public Player Cuffer { get; }
        public Player Target { get; }
        public bool Allowed { get; set; }
    }
    public class UnCuffEvent : CuffEvent
    {
        public UnCuffEvent(Player cuffer, Player target, bool allowed = true) : base(cuffer, target, allowed) { }
    }
    public class DamageEvent : EventArgs
    {
        private PlayerStats.HitInfo hitInformations;
        private DamageTypes.DamageType damageType = DamageTypes.None;
        public DamageEvent(Player attacker, Player target, PlayerStats.HitInfo hitInformations, bool allowed = true)
        {
            Attacker = attacker;
            Target = target;
            HitInformations = hitInformations;
            Allowed = allowed;
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
        public bool Allowed { get; set; }
    }
    public class DiesEvent : EventArgs
    {
        public DiesEvent(Player killer, Player target, PlayerStats.HitInfo hitInfo, bool allowed = true)
        {
            Killer = killer;
            Target = target;
            HitInfo = hitInfo;
            Allowed = allowed;
        }
        public Player Killer { get; }
        public Player Target { get; }
        public PlayerStats.HitInfo HitInfo { get; set; }
        public bool Allowed { get; set; }
    }
    public class InteractEvent : EventArgs
    {
        public InteractEvent(Player player) => Player = player;
        public Player Player { get; }
    }
    public class InteractDoorEvent : EventArgs
    {
        public InteractDoorEvent(Player player, Controllers.Door door, bool allowed = true)
        {
            Player = player;
            Door = door;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Controllers.Door Door { get; set; }
        public bool Allowed { get; set; }
    }
    public class InteractGeneratorEvent : EventArgs
    {
        public InteractGeneratorEvent(Player player, Controllers.Generator generator, GeneratorStatus status, bool allowed = true)
        {
            Player = player;
            Generator = generator;
            Status = status;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Controllers.Generator Generator { get; }
        public GeneratorStatus Status { get; }
        public bool Allowed { get; set; }
    }
    public class InteractLiftEvent : EventArgs
    {
        public InteractLiftEvent(Player player, Lift.Elevator elevator, Controllers.Lift lift, bool allowed = true)
        {
            Player = player;
            Elevator = elevator;
            Lift = lift;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Lift.Elevator Elevator { get; }
        public Controllers.Lift Lift { get; }
        public bool Allowed { get; set; }
    }
    public class InteractLockerEvent : EventArgs
    {
        public InteractLockerEvent(Player player, Locker locker, LockerChamber lockerChamber, byte lockerId, byte chamberId, bool allowed)
        {
            Player = player;
            Locker = locker;
            Chamber = lockerChamber;
            LockerId = lockerId;
            ChamberId = chamberId;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Locker Locker { get; }
        public LockerChamber Chamber { get; }
        public byte LockerId { get; }
        public byte ChamberId { get; }
        public bool Allowed { get; set; }
    }
    public class IcomSpeakEvent : EventArgs
    {
        public IcomSpeakEvent(Player player, bool allowed = true)
        {
            Player = player;
            Allowed = allowed;
        }
        public Player Player { get; }
        public bool Allowed { get; set; }
    }
    public class DroppingItemEvent : EventArgs
    {
        public DroppingItemEvent(Player player, Inventory.SyncItemInfo item, bool allowed = true)
        {
            Player = player;
            Item = item;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Inventory.SyncItemInfo Item { get; set; }
        public bool Allowed { get; set; }
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
        public PickupItemEvent(Player player, Pickup pickup, bool allowed = true) : base(player, pickup)
        {
            Allowed = allowed;
        }
        public bool Allowed { get; set; }
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
        public RechargeWeaponEvent(Player player, bool animationOnly, bool allowed = true)
        {
            Player = player;
            AnimationOnly = animationOnly;
            Allowed = allowed;
        }
        public Player Player { get; }
        public bool AnimationOnly { get; }
        public bool Allowed { get; set; }
    }
    public class ShootingEvent : EventArgs
    {
        public ShootingEvent(Player shooter, GameObject target, Vector3 position, bool allowed = true)
        {
            Shooter = shooter;
            Target = target;
            Position = position;
            Allowed = allowed;
        }
        public Player Shooter { get; }
        public GameObject Target { get; }
        public Vector3 Position { get; set; }
        public bool Allowed { get; set; }
    }
    public class SpeakEvent : EventArgs
    {
        public SpeakEvent(DissonanceUserSetup userSetup, bool icom, bool radio, bool mimicAs939, bool scpChat, bool ripChat, bool allowed = true)
        {
            UserSetup = userSetup;
            Intercom = icom;
            Radio = radio;
            MimicAs939 = mimicAs939;
            ScpChat = scpChat;
            RipChat = ripChat;
            Allowed = allowed;
        }
        public DissonanceUserSetup UserSetup { get; }
        public bool Intercom { get; }
        public bool Radio { get; }
        public bool MimicAs939 { get; }
        public bool ScpChat { get; }
        public bool RipChat { get; }
        public bool Allowed { get; set; }
    }
    public class RagdollSpawnEvent : EventArgs
    {
        public RagdollSpawnEvent(
            Player killer,
            Player owner,
            Controllers.Ragdoll ragdoll,
            bool allowed = true)
        {
            Killer = killer;
            Owner = owner;
            Ragdoll = ragdoll;
            Allowed = allowed;
        }
        public Player Killer { get; }
        public Player Owner { get; }
        public Controllers.Ragdoll Ragdoll { get; }
        public bool Allowed { get; set; }
    }
    public class MedicalUsedEvent : EventArgs
    {
        public MedicalUsedEvent(Player player, ItemType item)
        {
            Player = player;
            Item = item;
        }
        public Player Player { get; }
        public ItemType Item { get; }
    }
    public class MedicalUsingEvent : MedicalUsedEvent
    {
        public MedicalUsingEvent(Player player, ItemType item, float cooldown, bool allowed = true) : base(player, item)
        {
            Cooldown = cooldown;
            Allowed = allowed;
        }
        public float Cooldown { get; set; }
        public bool Allowed { get; set; }
    }
    public class MedicalStoppingEvent : MedicalUsingEvent
    {
        public MedicalStoppingEvent(Player player, ItemType item, float cooldown, bool allowed = true) : base(player, item, cooldown, allowed) { }
        public new float Cooldown => base.Cooldown;
    }
    public class SyncDataEvent : EventArgs
    {
        public SyncDataEvent(Player player, Vector2 speed, byte currentAnimation, bool allowed = true)
        {
            Player = player;
            Speed = speed;
            CurrentAnimation = currentAnimation;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector2 Speed { get; }
        public byte CurrentAnimation { get; set; }
        public bool Allowed { get; set; }
    }
    public class ThrowGrenadeEvent : EventArgs
    {
        public ThrowGrenadeEvent(Player player, GrenadeManager grenadeManager, int id, bool slow, double fuseTime, bool allowed = true)
        {
            Player = player;
            GrenadeManager = grenadeManager;
            Id = id;
            Slow = slow;
            FuseTime = fuseTime;
            Allowed = allowed;
        }
        public Player Player { get; }
        public GrenadeManager GrenadeManager { get; }
        public int Id { get; }
        public bool Slow { get; set; }
        public double FuseTime { get; set; }
        public bool Allowed { get; set; }
    }
    public class TeslaTriggerEvent : EventArgs
    {
        public TeslaTriggerEvent(Player player, bool inHurtingRange, bool triggerable = true)
        {
            Player = player;
            InHurtingRange = inHurtingRange;
            Triggerable = triggerable;
        }
        public Player Player { get; }
        public bool InHurtingRange { get; set; }
        public bool Triggerable { get; set; }
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