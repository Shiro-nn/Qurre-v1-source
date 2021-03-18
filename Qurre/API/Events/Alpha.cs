using System;
using System.Collections.Generic;
namespace Qurre.API.Events
{
    public class AlphaStartEvent : AlphaStopEvent
    {
        public AlphaStartEvent(Player player, bool allowed = true) : base(player, allowed) { }
    }
    public class AlphaStopEvent : EventArgs
    {
        public AlphaStopEvent(Player player, bool allowed = true)
        {
            Player = player ?? API.Server.Host;
            Allowed = allowed;
        }
        public Player Player { get; }
        public bool Allowed { get; set; }
    }
    public class EnableAlphaPanelEvent : EventArgs
    {
        public EnableAlphaPanelEvent(Player player, List<string> permissions, bool allowed = true)
        {
            Player = player;
            Permissions = permissions;
            Allowed = allowed;
        }
        public Player Player { get; }
        public List<string> Permissions { get; }
        public bool Allowed { get; set; }
    }
}