using Interactables.Interobjects.DoorUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Qurre.API.Events
{
    public class LCZDeconEvent : EventArgs
    {
        public LCZDeconEvent(bool allowed = true) => Allowed = allowed;
        public bool Allowed { get; set; }
    }
    public class AnnouncementDecontaminationEvent : EventArgs
    {
        private int id;
        public AnnouncementDecontaminationEvent(int announcementId, bool isGlobal, bool allowed = true)
        {
            Id = announcementId;
            IsGlobal = isGlobal;
            Allowed = allowed;
        }
        public int Id
        {
            get => id;
            set => id = Mathf.Clamp(value, 0, 6);
        }
        public bool IsGlobal { get; set; }
        public bool Allowed { get; set; }
    }
    public class MTFAnnouncementEvent : EventArgs
    {
        public MTFAnnouncementEvent(int scpsLeft, string unitName, int unitNumber, bool allowed = true)
        {
            ScpsLeft = scpsLeft;
            UnitName = unitName;
            UnitNumber = unitNumber;
            Allowed = allowed;
        }
        public int ScpsLeft { get; }
        public string UnitName { get; set; }
        public int UnitNumber { get; set; }
        public bool Allowed { get; set; }
    }
    public class NewBloodEvent : EventArgs
    {
        public NewBloodEvent(Player player, Vector3 position, int type, float multiplier, bool allowed = true)
        {
            Player = player;
            Position = position;
            Type = type;
            Multiplier = multiplier;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector3 Position { get; set; }
        public int Type { get; set; }
        public float Multiplier { get; set; }
        public bool Allowed { get; set; }
    }
    public class NewDecalEvent : EventArgs
    {
        public NewDecalEvent(Player owner, Vector3 position, Quaternion rotation, int type, bool allowed = true)
        {
            Owner = owner;
            Position = position;
            Rotation = rotation;
            Type = type;
            Allowed = allowed;
        }
        public Player Owner { get; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int Type { get; set; }
        public bool Allowed { get; set; }
    }
    public class GrenadeExplodeEvent : EventArgs
    {
        public GrenadeExplodeEvent(Player thrower, Dictionary<Player, float> targetToDamages, bool isFrag, GameObject grenade, bool allowed = true)
        {
            Thrower = thrower ?? Server.Host;
            TargetToDamages = targetToDamages;
            IsFrag = isFrag;
            Grenade = grenade;
            Allowed = allowed;
        }
        public Player Thrower { get; }
        public Dictionary<Player, float> TargetToDamages { get; }
        public List<Player> Targets => TargetToDamages.Keys.ToList();
        public bool IsFrag { get; }
        public GameObject Grenade { get; }
        public bool Allowed { get; set; }
    }
    public class SetSeedEvent : EventArgs
    {
        public SetSeedEvent(int seed) => Seed = seed;
        public int Seed { get; set; }
    }
    public class DoorDamageEvent : EventArgs
    {
        public DoorDamageEvent(Controllers.Door door, float hp, DoorDamageType damageType, bool allowed = true)
        {
            Door = door;
            Hp = hp;
            DamageType = damageType;
            Allowed = allowed;
        }
        public Controllers.Door Door { get; }
        public float Hp { get; }
        public DoorDamageType DamageType { get; }
        public bool Allowed { get; set; }
    }
    public class DoorLockEvent : EventArgs
    {
        public DoorLockEvent(Controllers.Door door, DoorLockReason reason, bool newState, bool allowed = true)
        {
            Door = door;
            Reason = reason;
            NewState = newState;
            Allowed = allowed;
        }
        public Controllers.Door Door { get; }
        public DoorLockReason Reason { get; }
        public bool NewState { get; }
        public bool Allowed { get; set; }
    }
}