using PlayableScps;
using Scp914;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Qurre.API.Events
{
    #region scp914
    public class SCP914
    {
        public class ActivatingEvent : EventArgs
        {
            public ActivatingEvent(ReferenceHub player, double duration, bool isAllowed = true)
            {
                Player = player;
                Duration = duration;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public double Duration { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class ChangeKnobEvent : EventArgs
        {
            private Scp914Knob knobSetting;
            public ChangeKnobEvent(ReferenceHub player, Scp914Knob knobSetting, bool isAllowed = true)
            {
                Player = player;
                KnobSetting = knobSetting;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public Scp914Knob KnobSetting
            {
                get => knobSetting;
                set => knobSetting = value > Scp914Machine.knobStateMax ? Scp914Machine.knobStateMin : value;
            }
            public bool IsAllowed { get; set; }
        }
        public class UpgradeEvent : EventArgs
        {
            public UpgradeEvent(Scp914Machine scp914, List<ReferenceHub> players, List<Pickup> items, Scp914Knob knob, bool isAllowed = true)
            {
                Scp914 = scp914;
                Players = players;
                Items = items;
                Knob = knob;
                IsAllowed = isAllowed;
            }
            public Scp914Machine Scp914 { get; }
            public List<ReferenceHub> Players { get; }
            public List<Pickup> Items { get; }
            public Scp914Knob Knob { get; set; }
            public bool IsAllowed { get; set; }
        }
    }
    #endregion
    #region scp106
    public class SCP106
    {
        public class PortalUsingEvent : EventArgs
        {
            public PortalUsingEvent(ReferenceHub player, Vector3 portalPosition, bool isAllowed = true)
            {
                Player = player;
                PortalPosition = portalPosition;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public Vector3 PortalPosition { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class PortalCreateEvent : EventArgs
        {
            public PortalCreateEvent(ReferenceHub player, Vector3 position, bool isAllowed = true)
            {
                Player = player;
                Position = position;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public Vector3 Position { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class ContainEvent : EventArgs
        {
            public ContainEvent(ReferenceHub player, bool isAllowed = true)
            {
                Player = player;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public bool IsAllowed { get; set; }
        }
        public class FemurBreakerEnterEvent : EventArgs
        {
            public FemurBreakerEnterEvent(ReferenceHub player, bool isAllowed = true)
            {
                Player = player;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public bool IsAllowed { get; set; }
        }
        public class PocketDimensionEnterEvent : EventArgs
        {
            public PocketDimensionEnterEvent(ReferenceHub player, Vector3 position, bool isAllowed = true)
            {
                Player = player;
                Position = position;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public Vector3 Position { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class PocketDimensionEscapeEvent : EventArgs
        {
            public PocketDimensionEscapeEvent(ReferenceHub player, Vector3 teleportPosition, bool isAllowed = true)
            {
                Player = player;
                TeleportPosition = teleportPosition;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public Vector3 TeleportPosition { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class PocketDimensionFailEscapeEvent : EventArgs
        {
            public PocketDimensionFailEscapeEvent(ReferenceHub player, PocketDimensionTeleport teleporter, bool isAllowed = true)
            {
                Player = player;
                Teleporter = teleporter;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public PocketDimensionTeleport Teleporter { get; }
            public bool IsAllowed { get; set; }
        }
    }
    #endregion
    #region scp096
    public class SCP096
    {
        public class EnrageEvent : EventArgs
        {
            public EnrageEvent(Scp096 scp096, ReferenceHub player, bool isAllowed = true)
            {
                Scp096 = scp096;
                Player = player;
                IsAllowed = isAllowed;
            }
            public Scp096 Scp096 { get; }
            public ReferenceHub Player { get; }
            public bool IsAllowed { get; set; }
        }
        public class CalmDownEvent : EnrageEvent
        {
            public CalmDownEvent(Scp096 scp096, ReferenceHub player, bool isAllowed = true)
                : base(scp096, player, isAllowed) { }
        }
        public class AddTargetEvent : EnrageEvent
        {
            public AddTargetEvent(Scp096 scp096, ReferenceHub player, bool isAllowed = true)
                : base(scp096, player, isAllowed) { }
        }
    }
    #endregion
    #region scp079
    public class SCP079
    {
        public class GeneratorActivateEvent : EventArgs
        {
            public GeneratorActivateEvent(Generator079 generator) => Generator = generator;
            public Generator079 Generator { get; }
        }
        public class RecontainEvent : EventArgs
        {
            public RecontainEvent(ReferenceHub target)
            {
                Target = target;
            }
            public ReferenceHub Target { get; }
        }
        public class GetEXPEvent : EventArgs
        {
            public GetEXPEvent(ReferenceHub player, ExpGainType type, float amount, bool isAllowed = true)
            {
                Player = player;
                Type = type;
                Amount = amount;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public ExpGainType Type { get; }
            public float Amount { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class GetLVLEvent : EventArgs
        {
            public GetLVLEvent(ReferenceHub player, int oldLevel, int newLevel, bool isAllowed = true)
            {
                Player = player;
                OldLevel = oldLevel;
                NewLevel = newLevel;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Player { get; }
            public int OldLevel { get; }
            public int NewLevel { get; set; }
            public bool IsAllowed { get; set; }
        }
    }
    #endregion
    #region scp049
    public class SCP049
    {
        public class StartRecallEvent : EventArgs
        {
            public StartRecallEvent(ReferenceHub scp049, ReferenceHub target, bool isAllowed = true)
            {
                Scp049 = scp049;
                Target = target;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Scp049 { get; }
            public ReferenceHub Target { get; }
            public bool IsAllowed { get; set; }
        }
        public class FinishRecallEvent : StartRecallEvent
        {
            public FinishRecallEvent(ReferenceHub scp049, ReferenceHub target, bool isAllowed = true)
                : base(scp049, target, isAllowed) { }
        }
    }
    #endregion
}