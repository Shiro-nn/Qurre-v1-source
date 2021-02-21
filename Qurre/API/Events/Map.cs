using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Qurre.API.Events
{
    public class LCZDeconEvent : EventArgs
    {
        public LCZDeconEvent(bool isAllowed = true) => IsAllowed = isAllowed;
        public bool IsAllowed { get; set; }
    }
    public class AnnouncementDecontaminationEvent : EventArgs
    {
        private int id;
        public AnnouncementDecontaminationEvent(int announcementId, bool isGlobal, bool isAllowed = true)
        {
            Id = announcementId;
            IsGlobal = isGlobal;
            IsAllowed = isAllowed;
        }
        public int Id
        {
            get => id;
            set => id = Mathf.Clamp(value, 0, 6);
        }
        public bool IsGlobal { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class MTFAnnouncementEvent : EventArgs
    {
        public MTFAnnouncementEvent(int scpsLeft, string unitName, int unitNumber, bool isAllowed = true)
        {
            ScpsLeft = scpsLeft;
            UnitName = unitName;
            UnitNumber = unitNumber;
            IsAllowed = isAllowed;
        }
        public int ScpsLeft { get; }
        public string UnitName { get; set; }
        public int UnitNumber { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class NewBloodEvent : EventArgs
    {
        public NewBloodEvent(Player player, Vector3 position, int type, float multiplier, bool isAllowed = true)
        {
            Player = player;
            Position = position;
            Type = type;
            Multiplier = multiplier;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public Vector3 Position { get; set; }
        public int Type { get; set; }
        public float Multiplier { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class NewDecalEvent : EventArgs
    {
        public NewDecalEvent(Player owner, Vector3 position, Quaternion rotation, int type, bool isAllowed = true)
        {
            Owner = owner;
            Position = position;
            Rotation = rotation;
            Type = type;
            IsAllowed = isAllowed;
        }
        public Player Owner { get; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public int Type { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class GrenadeExplodeEvent : EventArgs
    {
        public GrenadeExplodeEvent(Player thrower, Dictionary<Player, float> targetToDamages, bool isFrag, GameObject grenade, bool isAllowed = true)
        {
            Thrower = thrower ?? Map.Host;
            TargetToDamages = targetToDamages;
            IsFrag = isFrag;
            Grenade = grenade;
            IsAllowed = isAllowed;
        }
        public Player Thrower { get; }
        public Dictionary<Player, float> TargetToDamages { get; }
        public List<Player> Targets => TargetToDamages.Keys.ToList();
        public bool IsFrag { get; }
        public GameObject Grenade { get; }
        public bool IsAllowed { get; set; }
    }
}