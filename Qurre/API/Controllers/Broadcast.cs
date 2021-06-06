using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Qurre.API.Controllers
{
    internal class BC
    {
        private static global::Broadcast bc;
        internal static global::Broadcast BroadcastComponent
        {
            get
            {
                if (bc == null) bc = PlayerManager.localPlayer.GetComponent<global::Broadcast>();
                return bc;
            }
        }
    }
    public class Broadcast
    {
        private Player pl;
        private string msg;
        public Broadcast(Player player, string message, ushort time)
        {
            Message = message;
            Time = time;
            pl = player;
        }
        public float DisplayTime { get; internal set; } = float.MinValue;
        public string Message
        {
            get => msg;
            set
            {
                if (value != msg)
                {
                    msg = value;
                    if (Active) Update();
                }
            }
        }
        public ushort Time { get; }
        public bool Active { get; private set; }
        public void Start()
        {
            if (pl.Id == Server.Host.Id) { if (Map.Broadcasts.FirstOrDefault() != this) { return; } }
            if (pl.Id != Server.Host.Id) { if (pl.Broadcasts.FirstOrDefault() != this) { return; } }
            if (Active) return;
            Active = true;
            DisplayTime = UnityEngine.Time.time;
            if (pl.Id == Server.Host.Id) { BC.BroadcastComponent.RpcAddElement(Message, Time, global::Broadcast.BroadcastFlags.Normal); }
            else { BC.BroadcastComponent.TargetAddElement(pl.Scp079PlayerScript.connectionToClient, Message, Time, global::Broadcast.BroadcastFlags.Normal); }
            Timing.CallDelayed(Time, () => End());
        }
        public void Update()
        {
            var time = Time - (UnityEngine.Time.time - DisplayTime) + 1;
            if (pl.Id == Server.Host.Id)
            {
                BC.BroadcastComponent.RpcClearElements();
                BC.BroadcastComponent.RpcAddElement(Message, (ushort)time, global::Broadcast.BroadcastFlags.Normal);
            }
            else
            {
                BC.BroadcastComponent.TargetClearElements(pl.Scp079PlayerScript.connectionToClient);
                BC.BroadcastComponent.TargetAddElement(pl.Scp079PlayerScript.connectionToClient, Message, (ushort)time, global::Broadcast.BroadcastFlags.Normal);
            }
        }
        public void End()
        {
            if (!Active) return;
            Active = false;

            if (pl.Id == Server.Host.Id)
            {
                Map.Broadcasts.Remove(this);
                BC.BroadcastComponent.RpcClearElements();
                if (Map.Broadcasts.FirstOrDefault() != null) Map.Broadcasts.FirstOrDefault().Start();
                else foreach(Player pl in Player.List) if (pl.Broadcasts.FirstOrDefault() != null && !pl.Broadcasts.First().Active) pl.Broadcasts.First().Start();
            }
            else
            {
                pl.Broadcasts.Remove(this);
                BC.BroadcastComponent.TargetClearElements(pl.Scp079PlayerScript.connectionToClient);
                if (pl.Broadcasts.FirstOrDefault() != null) pl.Broadcasts.FirstOrDefault().Start();
            }
        }
    }
    public class ListBroadcasts
    {
        private Player _player;
        private List<Broadcast> bcs = new List<Broadcast>();
        public ListBroadcasts(Player player) => _player = player;
        public void Add(Broadcast bc, bool instant = false)
        {
            if (bc == null) return;
            if (instant)
            {
                var currentbc = bcs.FirstOrDefault();
                var list = new List<Broadcast>();
                list.Add(bc);
                list.AddRange(bcs);
                bcs = list;
                if (Map.Broadcasts.FirstOrDefault() != null && _player.Id != Server.Host.Id) BC.BroadcastComponent.TargetClearElements(_player.Connection);
                if (currentbc != null) currentbc.End();
                else bcs.First().Start();
            }
            else
            {
                bcs.Add(bc);
                if (!bcs.First().Active && !(Map.Broadcasts.FirstOrDefault() != null && Map.Broadcasts.First().Active)) bcs.First().Start();
            }
        }
        public void Remove(Broadcast bc)
        {
            if (bcs.Any(x => x == bc))
            {
                bcs.Remove(bc);
                if (bc.Active) bc.End();
            }
        }
        public void Clear()
        {
            if (bcs.Count < 1) return;
            var activebc = bcs.FirstOrDefault();
            bcs.Clear();
            activebc.End();
        }
        public List<Broadcast> List() => bcs;
        public bool Contains(Broadcast bc) => bcs.Contains(bc);
        public bool Any(Func<Broadcast, bool> func) => bcs.Any(func);
        public Broadcast First() => bcs.First();
        public Broadcast FirstOrDefault() => bcs.FirstOrDefault();
        public Broadcast FirstOrDefault(Func<Broadcast, bool> func) => bcs.FirstOrDefault(func);
    }
}