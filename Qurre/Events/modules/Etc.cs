using Qurre.API.Events;
using UnityEngine.SceneManagement;
using MapGeneration;
using UnityEngine;
using System.Collections.Generic;
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
            API.Map.AddObjects();
            if (API.Round.BotSpawned) Patches.Controllers.Bot.UnInitialize();
            API.Round.BotSpawned = false;
            API.Round.ForceEnd = false;
            RoundSummary.RoundLock = false;
            API.Round.ActiveGenerators = 0;
            if (Loader.AllUnits)
            {
                API.Round.AddUnit(API.Objects.TeamUnitType.ClassD, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.ChaosInsurgency, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.NineTailedFox, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Scientist, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Scp, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Tutorial, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.None, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
            }
            else if (Loader.OnlyTutorialUnit)
            {
                API.Round.AddUnit(API.Objects.TeamUnitType.Tutorial, $"<color=#31d400>Qurre v{PluginManager.Version}</color>");
            }
        }
        private static void FixOneSerial()
        {
            MEC.Timing.CallDelayed(5f, () =>
            {
                List<ushort> Serials = new();
                foreach (var pick in API.Map.Pickups)
                {
                    if (Serials.Contains(pick.Serial)) pick.Serial = 0;
                    Serials.Add(pick.Serial);
                }
            });
        }
        private static void FixFF(DamageProcessEvent ev)
        {
            if (API.Server.FriendlyFire) return;
            if (ev.Target == null || ev.Attacker == null) return;
            if (ev.Attacker.FriendlyFire) return;
            if (!Ally(ev.Target.Team, ev.Attacker.Team)) return;
            ev.FriendlyFire = true;
            static bool Ally(Team killer, Team target)
            {
                if (killer == target) return true;
                if ((killer == Team.MTF || killer == Team.RSC) && (target == Team.MTF || target == Team.RSC)) return true;
                if ((killer == Team.CHI || killer == Team.CDP) && (target == Team.CHI || target == Team.CDP)) return true;
                return false;
            }
        }
        private static void ChangeRole(RoleChangeEvent ev)
        {
            if (ev.Player?.IsHost != false || string.IsNullOrEmpty(ev.Player.UserId)) return;
            if (ev.NewRole == RoleType.Spectator) ev.Player.DropItems();
        }
        private static void FixItems(SpawnEvent ev)
        {
            if (ev.Player.Inventory.UserInventory.Items.Count == 0 && ev.Player.AllItems.Count != 0) ev.Player.ItemsValue.Clear();
        }
        private static void RoundRestart() => API.Map.ClearObjects();
        private static void SyncData(SyncDataEvent ev)
        {
            if (ev.Player != null && ev.Player.Escape != null && Vector3.Distance(ev.Player.Position, ev.Player.Escape.worldPosition) < Escape.radius)
                ev.Player.CheckEscape();
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
                    ev.ReplyMessage = "Example: bc time message.";
                    return;
                }
                if (!ushort.TryParse(ev.Args[0], out ushort num8) || num8 < 1)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "First argument must be a positive integer.";
                    return;
                }
                string text16 = ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + 2);
                API.Map.Broadcast(text16, System.Convert.ToUInt16(ev.Args[0])); ev.Success = true;
                ev.ReplyMessage = "Broadcast sent.";
            }
            else if (ev.Name == "pbc" && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Prefix = ev.Name.ToUpper();
                ev.Allowed = false;
                if (ev.Args.Length == 0)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Example: pbc id time message.";
                    return;
                }
                if (!ushort.TryParse(ev.Args[1], out ushort num8) || num8 < 1)
                {
                    ev.Success = false;
                    ev.ReplyMessage = "Second argument must be a positive integer.";
                    return;
                }
                string text16 = ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + ev.Args[1].Length + 3);
                foreach (string strs in ev.Args[0].Split('.'))
                {
                    if (ushort.TryParse(strs, out ushort pi) && pi > 0)
                    {
                        var pl = API.Player.Get(pi);
                        pl.Broadcast(text16, System.Convert.ToUInt16(ev.Args[1]));
                    }
                }
                ev.ReplyMessage = "Broadcast sent.";
            }
        }
    }
}