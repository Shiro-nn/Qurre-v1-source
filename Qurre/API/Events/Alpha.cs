using System;
using System.Collections.Generic;
namespace Qurre.API.Events
{
    public class AlphaStartEvent : AlphaStopEvent
    {
        public AlphaStartEvent(Player player, bool isAllowed = true)
            : base(player, isAllowed)
        {
        }
    }
    public class AlphaStopEvent : EventArgs
    {
        public AlphaStopEvent(Player player, bool isAllowed = true)
        {
            Player = player ?? API.Map.Host;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public bool IsAllowed { get; set; }
    }
    public class EnableAlphaPanelEvent : EventArgs
    {
        public EnableAlphaPanelEvent(Player player, List<string> permissions, bool isAllowed = true)
        {
            Player = player;
            Permissions = permissions;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public List<string> Permissions { get; }
        public bool IsAllowed { get; set; }
    }
}