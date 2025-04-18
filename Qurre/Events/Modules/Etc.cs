﻿using Qurre.API.Events;
using UnityEngine.SceneManagement;
using MapGeneration;
using UnityEngine;
using System.Collections.Generic;
using Qurre.API.Objects;
using System.Linq;
using Assets._Scripts.Dissonance;
using Mirror;
namespace Qurre.Events.Modules
{
    internal static class Etc
    {
        internal static void Load()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
            SeedSynchronizer.OnMapGenerated += Invoke.Map.Generated;
            Round.Waiting += Waiting;
            Player.RoleChange += ChangeRole;
            Round.Restart += RoundRestart;
            Server.SendingRA += FixRaBc;
            //Player.DamageProcess += FixFF;
            Round.Waiting += FixOneSerial;
            Player.ScpAttack += AntiCheat;

            MEC.Timing.RunCoroutine(CheckPlayersEscape());
            static IEnumerator<float> CheckPlayersEscape()
            {
                for (; ; )
                {
                    try
                    {
                        if (API.Round.Started)
                            foreach (var pl in API.Player.List)
                                try { CheckEscape(pl); } catch { }
                    }
                    catch { }
                    yield return MEC.Timing.WaitForSeconds(1);
                }
            }
        }
        private static void SceneUnloaded(Scene _)
        {
            API.Player.IdPlayers.Clear();
            API.Player.UserIDPlayers.Clear();
            API.Player.ArgsPlayers.Clear();
            API.Player.Dictionary.Clear();
            API.Map.ClearObjects();
        }
        private static void AntiCheat(ScpAttackEvent ev)
        {
            if (ev.Target.Team is Team.SCP) ev.Allowed = false;
        }
        private static void Waiting()
        {
            API.Audio._micro = Radio.comms.gameObject.AddComponent<API.Addons.Audio.Extensions.Microphone>();
            API.Server.Host.Dissonance.NetworkspeakingFlags = SpeakingFlags.IntercomAsHuman;

            if (API.Round.CurrentRound == 0)
                API.Addons.Prefabs.InitLate();

            API.Round.CurrentRound++;
            API.Map.AddObjects();

            if (API.Round.BotSpawned) Patches.Controllers.Bot.UnInitialize();
            API.Round.BotSpawned = false;
            API.Round.ForceEnd = false;

            RoundSummary.RoundLock = false;
            API.Round.ActiveGenerators = 0;

            if (Loader.AllUnits)
            {
                API.Round.AddUnit(TeamUnitType.ClassD, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(TeamUnitType.ChaosInsurgency, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(TeamUnitType.NineTailedFox, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(TeamUnitType.Scientist, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(TeamUnitType.Scp, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(TeamUnitType.Tutorial, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(TeamUnitType.None, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
            }
            else if (Loader.OnlyTutorialUnit)
            {
                API.Round.AddUnit(TeamUnitType.Tutorial, $"<color=#31d400>Qurre v{PluginManager.Version}</color>");
            }
        }
        private static void FixOneSerial()
        {
            MEC.Timing.CallDelayed(5f, () =>
            {
                List<ushort> Serials = new();
                foreach (API.Controllers.Items.Pickup pick in API.Map.Pickups)
                {
                    if (Serials.Contains(pick.Serial)) pick.Serial = 0;
                    Serials.Add(pick.Serial);
                }
            });
        }
        private static void FixFF(DamageProcessEvent ev)
        {
            /*
            if (API.Server.FriendlyFire || ev.Target is null || ev.Attacker is null || ev.Attacker.FriendlyFire)
                return;
            ev.FriendlyFire = ev.Target == ev.Attacker ? ev.PrimitiveType is not DamageTypesPrimitive.Explosion : ev.Target.Side == ev.Attacker.Side;
            */
        }
        private static void ChangeRole(RoleChangeEvent ev)
        {
            if ((ev.Player?.IsHost ?? true) is true || string.IsNullOrEmpty(ev.Player.UserId))
                return;
            if (ev.NewRole is RoleType.Spectator)
                ev.Player.DropItems();
        }
        private static void RoundRestart() => API.Map.ClearObjects();
        private static void CheckEscape(API.Player pl)
        {
            if (pl.Escape is not null && Vector3.Distance(pl.Position, pl.Escape.worldPosition) < Escape.radius)
                pl.CheckEscape();
        }
        private static void FixRaBc(SendingRAEvent ev)
        {
            if ((ev.Name == "bc" || ev.Name == "broadcast") && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Prefix = ev.Name.ToUpper();
                ev.Allowed = false;
                if (ev.Args.Length == 0)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Use: bc [Time] [Message].";
                    return;
                }
                if (!ushort.TryParse(ev.Args[0], out ushort time) || time < 1)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "First argument must be a positive integer.";
                    return;
                }
                API.Map.Broadcast(ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + 2), time);
                ev.Success = true;
                ev.ReplyMessage = "Broadcast sent.";
            }
            else if (ev.Command.StartsWith("@") && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.AdminChat))
            {
                ev.Allowed = false;
                string content = $"<color=#ffa500>[Admin Chat]</color> <color=#008000>{ev.Command.Substring(1)} ~ {ev.CommandSender.Nickname}</color>";
                foreach (API.Player pl in API.Player.List.Where(x => PermissionsHandler.IsPermitted(x.Sender.Permissions, PlayerPermissions.AdminChat)))
                {
                    pl.Broadcast(content, 5, true);
                    pl.SendConsoleMessage(content, "green");
                }
                content = null;
            }
            else if (ev.Name == "pbc" && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Prefix = ev.Name.ToUpper();
                ev.Allowed = false;
                if (ev.Args.Length < 3)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Use: pbc [Id] [Time] [Message].";
                    return;
                }
                if (!ushort.TryParse(ev.Args[1], out ushort time) || time < 1)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Second argument must be a positive integer.";
                    return;
                }
                string content = ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + ev.Args[1].Length + 3);
                foreach (string strs in ev.Args[0].Split('.'))
                {
                    if (ushort.TryParse(strs, out ushort pi) && pi > 0)
                    {
                        API.Player pl = API.Player.Get(pi);
                        if (pl is not null) pl.Broadcast(content, time);
                    }
                }
                ev.ReplyMessage = "Broadcast sent.";
                content = null;
            }
        }
    }
}