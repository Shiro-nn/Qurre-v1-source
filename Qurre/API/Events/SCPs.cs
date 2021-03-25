using PlayableScps;
using Scp914;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace Qurre.API.Events
{
    #region scp914
    public class ActivatingEvent : EventArgs
    {
        public ActivatingEvent(Player player, double duration, bool allowed = true)
        {
            Player = player;
            Duration = duration;
            Allowed = allowed;
        }
        public Player Player { get; }
        public double Duration { get; set; }
        public bool Allowed { get; set; }
    }
    public class ChangeKnobEvent : EventArgs
    {
        private Scp914Knob knobSetting;
        public ChangeKnobEvent(Player player, Scp914Knob knobSetting, bool allowed = true)
        {
            Player = player;
            KnobSetting = knobSetting;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Scp914Knob KnobSetting
        {
            get => knobSetting;
            set => knobSetting = value > Scp914Machine.knobStateMax ? Scp914Machine.knobStateMin : value;
        }
        public bool Allowed { get; set; }
    }
    public class UpgradeEvent : EventArgs
    {
        public UpgradeEvent(Scp914Machine scp914, List<Player> players, List<Pickup> items, Scp914Knob knob, bool allowed = true)
        {
            Scp914 = scp914;
            Players = players;
            Items = items;
            Knob = knob;
            Allowed = allowed;
        }
        public Scp914Machine Scp914 { get; }
        public List<Player> Players { get; }
        public List<Pickup> Items { get; }
        public Scp914Knob Knob { get; set; }
        public bool Allowed { get; set; }
    }
    #endregion
    #region scp173
    public class BlinkEvent : EventArgs
    {
        public BlinkEvent(HashSet<Player> players)
        {
            Players = players;
        }
        public HashSet<Player> Players { get; }
    }
    #endregion
    #region scp106
    public class PortalUsingEvent : EventArgs
    {
        public PortalUsingEvent(Player player, Vector3 portalPosition, bool allowed = true)
        {
            Player = player;
            PortalPosition = portalPosition;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector3 PortalPosition { get; set; }
        public bool Allowed { get; set; }
    }
    public class PortalCreateEvent : EventArgs
    {
        public PortalCreateEvent(Player player, Vector3 position, bool allowed = true)
        {
            Player = player;
            Position = position;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector3 Position { get; set; }
        public bool Allowed { get; set; }
    }
    public class ContainEvent : EventArgs
    {
        public ContainEvent(Player player, bool allowed = true)
        {
            Player = player;
            Allowed = allowed;
        }
        public Player Player { get; }
        public bool Allowed { get; set; }
    }
    public class FemurBreakerEnterEvent : EventArgs
    {
        public FemurBreakerEnterEvent(Player player, bool allowed = true)
        {
            Player = player;
            Allowed = allowed;
        }
        public Player Player { get; }
        public bool Allowed { get; set; }
    }
    public class PocketDimensionEnterEvent : EventArgs
    {
        public PocketDimensionEnterEvent(Player player, Vector3 position, bool allowed = true)
        {
            Player = player;
            Position = position;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector3 Position { get; set; }
        public bool Allowed { get; set; }
    }
    public class PocketDimensionEscapeEvent : EventArgs
    {
        public PocketDimensionEscapeEvent(Player player, Vector3 teleportPosition, bool allowed = true)
        {
            Player = player;
            TeleportPosition = teleportPosition;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector3 TeleportPosition { get; set; }
        public bool Allowed { get; set; }
    }
    public class PocketDimensionFailEscapeEvent : EventArgs
    {
        public PocketDimensionFailEscapeEvent(Player player, PocketDimensionTeleport teleporter, bool allowed = true)
        {
            Player = player;
            Teleporter = teleporter;
            Allowed = allowed;
        }
        public Player Player { get; }
        public PocketDimensionTeleport Teleporter { get; }
        public bool Allowed { get; set; }
    }
    #endregion
    #region scp096
    public class EnrageEvent : EventArgs
    {
        public EnrageEvent(Scp096 scp096, Player player, bool allowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Allowed = allowed;
        }
        public Scp096 Scp096 { get; }
        public Player Player { get; }
        public bool Allowed { get; set; }
    }
    public class CalmDownEvent : EnrageEvent
    {
        public CalmDownEvent(Scp096 scp096, Player player, bool allowed = true) : base(scp096, player, allowed) { }
    }
    public class AddTargetEvent : EventArgs
    {
        public AddTargetEvent(Scp096 scp096, Player player, Player target, bool allowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Target = target;
            Allowed = allowed;
        }
        public Scp096 Scp096 { get; }
        public Player Player { get; }
        public Player Target { get; }
        public bool Allowed { get; set; }
    }
    #endregion
    #region scp079
    public class GeneratorActivateEvent : EventArgs
    {
        public GeneratorActivateEvent(Controllers.Generator generator) => Generator = generator;
        public Controllers.Generator Generator { get; }
    }
    public class GetEXPEvent : EventArgs
    {
        public GetEXPEvent(Player player, ExpGainType type, float amount, bool allowed = true)
        {
            Player = player;
            Type = type;
            Amount = amount;
            Allowed = allowed;
        }
        public Player Player { get; }
        public ExpGainType Type { get; }
        public float Amount { get; set; }
        public bool Allowed { get; set; }
    }
    public class GetLVLEvent : EventArgs
    {
        public GetLVLEvent(Player player, int oldLevel, int newLevel, bool allowed = true)
        {
            Player = player;
            OldLevel = oldLevel;
            NewLevel = newLevel;
            Allowed = allowed;
        }
        public Player Player { get; }
        public int OldLevel { get; }
        public int NewLevel { get; set; }
        public bool Allowed { get; set; }
    }
    #endregion
    #region scp049
    public class StartRecallEvent : EventArgs
    {
        public StartRecallEvent(Player scp049, Player target, bool allowed = true)
        {
            Scp049 = scp049;
            Target = target;
            Allowed = allowed;
        }
        public Player Scp049 { get; }
        public Player Target { get; }
        public bool Allowed { get; set; }
    }
    public class FinishRecallEvent : StartRecallEvent
    {
        public FinishRecallEvent(Player scp049, Player target, bool allowed = true) : base(scp049, target, allowed) { }
    }
    #endregion
}