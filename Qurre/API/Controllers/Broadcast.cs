﻿using MEC;
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
        public void Start(Player player)
        {
            if (player.Broadcasts.FirstOrDefault() != this) return;
            if (Active) return;
            Active = true;
            DisplayTime = UnityEngine.Time.time;
            BC.BroadcastComponent.TargetAddElement(pl.Scp079PlayerScript.connectionToClient, Message, Time, global::Broadcast.BroadcastFlags.Normal);
            Timing.CallDelayed(Time, () => End());
        }
        public void Update()
        {
            var time = Time - (UnityEngine.Time.time - DisplayTime) + 1;
            BC.BroadcastComponent.TargetClearElements(pl.Scp079PlayerScript.connectionToClient);
            BC.BroadcastComponent.TargetAddElement(pl.Scp079PlayerScript.connectionToClient, Message, (ushort)time, global::Broadcast.BroadcastFlags.Normal);
        }
        public void End()
        {
            if (!Active) return;
            Active = false;
            pl.Broadcasts.Remove(this);
            BC.BroadcastComponent.TargetClearElements(pl.Scp079PlayerScript.connectionToClient);
            if (pl.Broadcasts.FirstOrDefault() != null) pl.Broadcasts.FirstOrDefault().Start(pl);
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
                if (currentbc != null) currentbc.End();
                else bcs.First().Start(_player);
            }
            else
            {
                bcs.Add(bc);
                if (!bcs.First().Active) bcs.First().Start(_player);
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
        public Broadcast FirstOrDefault() => bcs.FirstOrDefault();
        public Broadcast FirstOrDefault(Func<Broadcast, bool> func) => bcs.FirstOrDefault(func);
    }


    public class MapBroadcast
    {
        private string msg;
        public MapBroadcast(Player player, string message, ushort time)
        {
            Message = message;
            Time = time;
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
            if (Map.Broadcasts.FirstOrDefault() != this) return;
            if (Active) return;
            Active = true;
            DisplayTime = UnityEngine.Time.time;
            BC.BroadcastComponent.RpcAddElement(Message, Time, global::Broadcast.BroadcastFlags.Normal);
            Timing.CallDelayed(Time, () => End());
        }
        public void Update()
        {
            var time = Time - (UnityEngine.Time.time - DisplayTime) + 1;
            BC.BroadcastComponent.RpcClearElements();
            BC.BroadcastComponent.RpcAddElement(Message, (ushort)time, global::Broadcast.BroadcastFlags.Normal);
        }
        public void End()
        {
            if (!Active) return;
            Active = false;
            Map.Broadcasts.Remove(this);
            BC.BroadcastComponent.RpcClearElements();
            if (Map.Broadcasts.FirstOrDefault() != null) Map.Broadcasts.FirstOrDefault().Start();
        }
    }
    public class MapListBroadcasts
    {
        private List<MapBroadcast> bcs = new List<MapBroadcast>();
        public MapListBroadcasts() { }
        public void Add(MapBroadcast bc, bool instant = false)
        {
            if (bc == null) return;
            if (instant)
            {
                var currentbc = bcs.FirstOrDefault();
                var list = new List<MapBroadcast>();
                list.Add(bc);
                list.AddRange(bcs);
                bcs = list;
                if (currentbc != null) currentbc.End();
                else bcs.First().Start();
            }
            else
            {
                bcs.Add(bc);
                if (!bcs.First().Active) bcs.First().Start();
            }
        }
        public void Remove(MapBroadcast bc)
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
        public List<MapBroadcast> List() => bcs;
        public bool Contains(MapBroadcast bc) => bcs.Contains(bc);
        public bool Any(Func<MapBroadcast, bool> func) => bcs.Any(func);
        public MapBroadcast FirstOrDefault() => bcs.FirstOrDefault();
        public MapBroadcast FirstOrDefault(Func<MapBroadcast, bool> func) => bcs.FirstOrDefault(func);
    }
}