using System;
using System.Collections.Generic;
namespace Qurre.API.Events.Alpha
{
    public class StartEvent : StopEvent
    {
        public StartEvent(ReferenceHub player, bool isAllowed = true)
            : base(player, isAllowed)
        {
        }
    }
    public class StopEvent : EventArgs
    {
        public StopEvent(ReferenceHub player, bool isAllowed = true)
        {
            Player = player ?? API.Map.Host;
            IsAllowed = isAllowed;
        }
        public ReferenceHub Player { get; }
        public bool IsAllowed { get; set; }
    }
    public class EnablePanelEvent : EventArgs
    {
        public EnablePanelEvent(ReferenceHub player, List<string> permissions, bool isAllowed = true)
        {
            Player = player;
            Permissions = permissions;
            IsAllowed = isAllowed;
        }
        public ReferenceHub Player { get; }
        public List<string> Permissions { get; }
        public bool IsAllowed { get; set; }
    }
}