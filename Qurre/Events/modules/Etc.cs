using Qurre.API.Events;
using UnityEngine.SceneManagement;
using MapGeneration;
using Interactables.Interobjects.DoorUtils;
using System.Linq;
using System.Collections.Generic;
using Dissonance.Config;
using Dissonance;
using Dissonance.Networking.Client;
namespace Qurre.Events.modules
{
    internal class Etc
    {
        internal static void Load()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
            SeedSynchronizer.OnMapGenerated += Invoke.Map.Generated;
            Round.WaitingForPlayers += WaitingForPlayers;
            Player.RoleChange += ChangeRole;
            Round.Restart += RoundRestart;
            Server.SendingRA += FixRaBc;
            Map.DoorLock += Fix079;
            MEC.Timing.RunCoroutine(UpdateAudioClient());
        }
        private static void SceneUnloaded(Scene _)
        {
            API.Player.IdPlayers.Clear();
            API.Player.UserIDPlayers.Clear();
            API.Player.Dictionary.Clear();
            API.Map.ClearObjects();
        }
        private static void WaitingForPlayers()
        {
            API.Round.ForceEnd = false;
            RoundSummary.RoundLock = false;
            API.Round.ActiveGenerators = 0;
            if (Plugin.Config.GetBool("Qurre_AllUnit", false))
            {
                API.Round.AddUnit(API.Objects.TeamUnitType.ClassD, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.ChaosInsurgency, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.NineTailedFox, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Scientist, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Scp, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Tutorial, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.None, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
            }
            else if (Plugin.Config.GetBool("Qurre_OnlyTutorialUnit", false))
            {
                API.Round.AddUnit(API.Objects.TeamUnitType.Tutorial, $"<color=#31d400>Qurre v{PluginManager.Version}</color>");
            }
        }
        private static void ChangeRole(RoleChangeEvent ev)
        {
            if (ev.Player?.IsHost != false || string.IsNullOrEmpty(ev.Player.UserId)) return;
            if (ev.NewRole == RoleType.Spectator) ev.Player.Inventory.ServerDropAll();
        }
        private static void RoundRestart() => API.Map.ClearObjects();
        private static IEnumerator<float> UpdateAudioClient()
        {
            for (; ; )
            {
                yield return float.NegativeInfinity;
                if (QurreModLoader.Audio.client != null && !QurreModLoader.Audio.client._disconnected)
                {
                    int num;
                    for (int i = 0; i < DebugSettings.Instance._levels.Count; i = num + 1)
                    {
                        DebugSettings.Instance._levels[i] = LogLevel.Trace;
                        num = i;
                    }
                    if (QurreModLoader.Audio.client.Update() == ClientStatus.Error)
                    {
                        if (Log.debug) Log.Error("Audio Client caused an error.");
                    }
                }
            }
        }
        private static void FixRaBc(SendingRAEvent ev)
        {
            if ((ev.Name == "bc" || ev.Name == "broadcast") && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Allowed = false;
                if (ev.Args.Length == 0)
                {
                    ev.CommandSender.RaReply(ev.Name.ToUpper() + "#Example: bc time message.", false, true, "");
                    return;
                }
                if (!ushort.TryParse(ev.Args[0], out ushort num8) || num8 < 1)
                {
                    ev.CommandSender.RaReply(ev.Name.ToUpper() + "#First argument must be a positive integer.", false, true, "");
                    return;
                }
                string text16 = ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + 2);
                API.Map.Broadcast(text16, System.Convert.ToUInt16(ev.Args[0])); ev.Success = true;
                ev.CommandSender.RaReply(ev.Name.ToUpper() + "#Broadcast sent.", false, true, "");
            }
            else if (ev.Name == "pbc" && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Allowed = false;
                if (ev.Args.Length == 0)
                {
                    ev.CommandSender.RaReply(ev.Name.ToUpper() + "#Example: pbc id time message.", false, true, "");
                    return;
                }
                if (!ushort.TryParse(ev.Args[0], out ushort pi) || pi < 1)
                {
                    ev.CommandSender.RaReply(ev.Name.ToUpper() + "#First argument must be a positive integer.", false, true, "");
                    return;
                }
                if (!ushort.TryParse(ev.Args[1], out ushort num8) || num8 < 1)
                {
                    ev.CommandSender.RaReply(ev.Name.ToUpper() + "#Second argument must be a positive integer.", false, true, "");
                    return;
                }
                string text16 = ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + ev.Args[1].Length + 3);
                var pl = API.Player.Get(pi);
                pl.Broadcast(text16, System.Convert.ToUInt16(ev.Args[1])); ev.Success = true;
                ev.CommandSender.RaReply(ev.Name.ToUpper() + "#Broadcast sent.", false, true, "");
            }
        }
        private static void Fix079(DoorLockEvent ev)
        {
            if (ev.Door == null) return;
            if (ev.Reason == DoorLockReason.NoPower && ev.NewState)
            {
                MEC.Timing.CallDelayed(3f, () => ev.Door.DoorVariant.ServerChangeLock(DoorLockReason.NoPower, false));
                if (API.Round.ActiveGenerators > 4) foreach (API.Player pl in API.Player.List.Where(x => x.Role == RoleType.Scp079)) pl.Kill(DamageTypes.Recontainment);
            }
        }
    }
}