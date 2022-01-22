using Respawning;
using Subtitles;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Networking;
namespace Qurre.API.Controllers
{
    public class Cassie
    {
        public Cassie(string message, bool makeHold, bool makeNoise)
        {
            Message = message;
            Hold = makeHold;
            Noise = makeNoise;
        }
        public Cassie(string message, bool makeHold, bool makeNoise, List<SubtitlePart> subtitles)
        {
            Message = message;
            Hold = makeHold;
            Noise = makeNoise;
            if (subtitles is not null && subtitles != default) Subtitles = subtitles;
        }
        public string Message = "";
        public bool Hold = false;
        public bool Noise = false;
        public readonly List<SubtitlePart> Subtitles = new();
        public bool Active { get; private set; }
        public void Send()
        {
            if (Active) return;
            Active = true;
            RespawnEffectsController.PlayCassieAnnouncement(Message, Hold, Noise);
            if (Subtitles.Count == 0) return;
            new SubtitleMessage(Subtitles.ToArray()).SendToAuthenticated();
        }
        internal static void End()
        {
            if (Map.Cassies.FirstOrDefault() != null) Map.Cassies.FirstOrDefault().Send();
        }
        public static bool Lock { get; set; }
        public static void Send(string msg, bool makeHold = false, bool makeNoise = false, bool instant = false, List<SubtitlePart> subtitles = null) =>
            Map.Cassies.Add(new Cassie(msg, makeHold, makeNoise, subtitles), instant);
        public static void Send(string msg, List<SubtitlePart> subtitles, bool makeHold = false, bool makeNoise = false, bool instant = false) =>
            Map.Cassies.Add(new Cassie(msg, makeHold, makeNoise, subtitles), instant);
    }
    public class CassieList
    {
        private List<Cassie> Cassies = new();
        public void Add(Cassie bc, bool instant = false)
        {
            if (bc == null) return;
            if (instant)
            {
                List<Cassie> list = new();
                list.Add(bc);
                list.AddRange(Cassies);
                Cassies = list;
                Cassies.First().Send();
            }
            else
            {
                Cassies.Add(bc);
                if (!Cassies.First().Active) Cassies.First().Send();
            }
        }
        public void Remove(Cassie bc)
        {
            if (Cassies.Any(x => x == bc))
                Cassies.Remove(bc);
        }
        public void Clear()
        {
            if (Cassies.Count < 1) return;
            Cassies.Clear();
        }
        public List<Cassie> List() => Cassies;
        public bool Contains(Cassie bc) => Cassies.Contains(bc);
        public bool Any(Func<Cassie, bool> func) => Cassies.Any(func);
        public Cassie FirstOrDefault() => Cassies.FirstOrDefault();
    }
}