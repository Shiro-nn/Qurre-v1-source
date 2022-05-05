using MapGeneration;
using Qurre.API.Events;
using Qurre.API.Objects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            Player.SyncData += SyncData;
            Server.SendingRA += FixRaBc;
            Player.Spawn += FixItems;
            Player.DamageProcess += FixFF;
            Round.Waiting += FixOneSerial;
        }

        private static void SceneUnloaded(Scene _)
        {
            API.Player.IdPlayers.Clear();
            API.Player.UserIDPlayers.Clear();
            API.Player.ArgsPlayers.Clear();
            API.Player.Dictionary.Clear();
            API.Map.ClearObjects();
        }

        private static void Waiting()
        {
            if (API.Round.CurrentRound is 0)
            {
                API.Addons.Prefabs.InitLate();
            }

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

        private static void FixOneSerial() =>
            MEC.Timing.CallDelayed(5f, () =>
            {
                List<ushort> Serials = new();
                foreach (API.Controllers.Items.Pickup pick in API.Map.Pickups)
                {
                    if (Serials.Contains(pick.Serial))
                        pick.Serial = 0;

                    Serials.Add(pick.Serial);
                }
            });

        private static void FixFF(DamageProcessEvent ev)
        {
            if (API.Server.FriendlyFire || ev.Target is null || ev.Attacker is null || ev.Attacker.FriendlyFire)
                return;

            ev.FriendlyFire = ev.Target.Side == ev.Attacker.Side;
        }

        private static void ChangeRole(RoleChangeEvent ev)
        {
            if ((ev.Player?.IsHost ?? true) is true || string.IsNullOrEmpty(ev.Player.UserId))
                return;

            if (ev.NewRole == RoleType.Spectator)
                ev.Player.DropItems();
        }

        private static void FixItems(SpawnEvent ev)
        {
            if (ev.Player.AllItems.Count is not 0 && ev.Player.Inventory.UserInventory.Items.Count is 0)
                ev.Player.ItemsValue.Clear();
        }

        private static void RoundRestart() => API.Map.ClearObjects();

        private static void SyncData(SyncDataEvent ev)
        {
            if (ev.Player is not null && ev.Player.Escape is not null && Vector3.Distance(ev.Player.Position, ev.Player.Escape.worldPosition) < Escape.radius)
                ev.Player.CheckEscape();
        }

        private static void FixRaBc(SendingRAEvent ev)
        {
            if ((ev.Name is "bc" || ev.Name is "broadcast") && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Prefix = ev.Name.ToUpper();
                ev.Allowed = false;

                if (ev.Args.Length is 0)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Using: bc [Time] [Message].";
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
                foreach (API.Player ply in API.Player.List.Where(x => PermissionsHandler.IsPermitted(x.Sender.Permissions, PlayerPermissions.AdminChat)))
                {
                    ply.Broadcast($"<color=#ffa500>[Admin Chat]</color> <color=#008000>{ev.Command.Substring(1)} ~ {ev.CommandSender.Nickname}</color>", 5, true);
                    ply.SendConsoleMessage($"<color=#ffa500>[Admin Chat]</color> <color=#008000>{ev.Command.Substring(1)} ~ {ev.CommandSender.Nickname}</color>", "green");
                }
            }
            else if (ev.Name is "pbc" && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Prefix = ev.Name.ToUpper();
                ev.Allowed = false;

                if (ev.Args.Length < 3)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Using: pbc [Id] [Time] [Message].";
                    return;
                }

                if (!ushort.TryParse(ev.Args[0], out ushort time) || time < 1)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Second argument must be a positive integer.";
                    return;
                }

                foreach (string strs in ev.Args[0].Split('.'))
                {
                    API.Player.Get(strs).Broadcast(ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + ev.Args[1].Length + 3), time);
                }

                ev.ReplyMessage = "Broadcast sent.";
            }
        }
    }
}