using InventorySystem.Items.Pickups;
using PlayableScps;
using Qurre.API.Controllers;
using Qurre.API.Controllers.Items;
using Scp914;
using System;
using System.Collections.Generic;
using UnityEngine;
using Scp096 = PlayableScps.Scp096;
namespace Qurre.API.Events
{
    #region scp914
    public class ActivatingEvent : EventArgs
    {
        public ActivatingEvent(Player player, float duration, bool allowed = true)
        {
            Player = player;
            Duration = duration;
            Allowed = allowed;
        }
        public Player Player { get; }
        public float Duration { get; set; }
        public bool Allowed { get; set; }
    }
    public class KnobChangeEvent : EventArgs
    {
        public KnobChangeEvent(Player player, Scp914KnobSetting setting, bool allowed = true)
        {
            Player = player;
            Setting = setting;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Scp914KnobSetting Setting { get; set; }
        public bool Allowed { get; set; }
    }
    public class UpgradeEvent : EventArgs
    {
        public UpgradeEvent(List<Player> players, List<ItemPickupBase> items, Vector3 moveVector, Scp914Mode mode, Scp914KnobSetting setting, bool allowed = true)
        {
            Players = players;
            Items = items;
            Move = moveVector;
            Mode = mode;
            Setting = setting;
            Allowed = allowed;
        }
        public List<Player> Players { get; }
        public List<ItemPickupBase> Items { get; }
        public Vector3 Move { get; set; }
        public Scp914Mode Mode { get; set; }
        public Scp914KnobSetting Setting { get; set; }
        public bool Allowed { get; set; }
    }
    public class UpgradePlayerEvent : EventArgs
    {
        public UpgradePlayerEvent(Player player, bool upgradeInventory, bool heldOnly, Vector3 moveVector, Scp914KnobSetting setting, bool allowed = true)
        {
            Player = player;
            UpgradeInventory = upgradeInventory;
            HeldOnly = heldOnly;
            Move = moveVector;
            Setting = setting;
            Allowed = allowed;
        }
        public Player Player { get; }
        public bool UpgradeInventory { get; set; }
        public bool HeldOnly { get; set; }
        public Vector3 Move { get; set; }
        public Scp914KnobSetting Setting { get; set; }
        public bool Allowed { get; set; }
    }
    public class UpgradePickupEvent : EventArgs
    {
        public UpgradePickupEvent(ItemPickupBase pickup, bool upgradeDropped, Vector3 moveVector, Scp914KnobSetting setting, bool allowed = true)
        {
            Pickup = pickup;
            UpgradeDropped = upgradeDropped;
            Move = moveVector;
            Setting = setting;
            Allowed = allowed;
        }
        public ItemPickupBase Pickup { get; }
        public bool UpgradeDropped { get; set; }
        public Vector3 Move { get; set; }
        public Scp914KnobSetting Setting { get; set; }
        public bool Allowed { get; set; }
    }
    public class UpgradedItemInventoryEvent : EventArgs
    {
        public UpgradedItemInventoryEvent(Item item, Player player, Scp914KnobSetting setting, bool allowed = true)
        {
            Item = item;
            Player = player;
            Setting = setting;
            Allowed = allowed;
        }
        public Item Item { get; }
        public Player Player { get; set; }
        public Scp914KnobSetting Setting { get; set; }
        public bool Allowed { get; set; }
    }
    public class UpgradedItemPickupEvent : EventArgs
    {
        public UpgradedItemPickupEvent(Pickup pickup, Vector3 pos, Scp914KnobSetting setting, bool allowed = true)
        {
            Pickup = pickup;
            Position = pos;
            Setting = setting;
            Allowed = allowed;
        }
        public Pickup Pickup { get; }
        public Vector3 Position { get; set; }
        public Scp914KnobSetting Setting { get; set; }
        public bool Allowed { get; set; }
    }
    #endregion
    #region scp173
    public class BlinkEvent : EventArgs
    {
        public BlinkEvent(Player scp, Vector3 pos, bool allowed = true)
        {
            Scp = scp;
            Position = pos;
            Allowed = allowed;
        }
        public Player Scp { get; }
        public Vector3 Position { get; set; }
        public bool Allowed { get; set; }
    }
    public class TantrumPlaceEvent : EventArgs
    {
        public TantrumPlaceEvent(Player scp, float cooldown = 30, bool allowed = true)
        {
            Scp = scp;
            Cooldown = cooldown;
            Allowed = allowed;
        }
        public Player Scp { get; }
        public float Cooldown { get; set; }
        public bool Allowed { get; set; }
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
    public class PocketEnterEvent : EventArgs
    {
        public PocketEnterEvent(Player player, Vector3 position, bool allowed = true)
        {
            Player = player;
            Position = position;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector3 Position { get; set; }
        public bool Allowed { get; set; }
    }
    public class PocketEscapeEvent : EventArgs
    {
        public PocketEscapeEvent(Player player, Vector3 teleportPosition, bool allowed = true)
        {
            Player = player;
            TeleportPosition = teleportPosition;
            Allowed = allowed;
        }
        public Player Player { get; }
        public Vector3 TeleportPosition { get; set; }
        public bool Allowed { get; set; }
    }
    public class PocketFailEscapeEvent : EventArgs
    {
        public PocketFailEscapeEvent(Player player, PocketDimensionTeleport teleporter, bool allowed = true)
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
    public class WindupEvent : EventArgs
    {
        public WindupEvent(Scp096 scp096, Player player, bool force, bool allowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Force = force;
            Allowed = allowed;
        }
        public Scp096 Scp096 { get; }
        public Player Player { get; }
        public bool Force { get; }
        public bool Allowed { get; set; }
    }
    public class CalmDownEvent : EventArgs
    {
        public CalmDownEvent(Scp096 scp096, Player player, bool allowed = true)
        {
            Scp096 = scp096;
            Player = player;
            Allowed = allowed;
        }
        public Scp096 Scp096 { get; }
        public Player Player { get; }
        public bool Allowed { get; set; }
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
        public GeneratorActivateEvent(Controllers.Generator generator, bool allowed = true)
        {
            Generator = generator;
            Allowed = allowed;
        }
        public Controllers.Generator Generator { get; }
        public bool Allowed { get; set; }
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
    public class FinishRecallEvent : EventArgs
    {
        public FinishRecallEvent(Player scp049, Player target, bool allowed = true)
        {
            Scp049 = scp049;
            Target = target;
            Allowed = allowed;
        }
        public Player Scp049 { get; }
        public Player Target { get; }
        public bool Allowed { get; set; }
    }
    #endregion
}