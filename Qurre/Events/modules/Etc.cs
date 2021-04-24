using Qurre.API.Events;
using UnityEngine.SceneManagement;
using MapGeneration;
namespace Qurre.Events.modules
{
    internal class Etc
    {
        internal static void Load()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
            SeedSynchronizer.OnMapGenerated += Map.generated;
            Round.WaitingForPlayers += WaitingForPlayers;
            Player.RoleChange += ChangeRole;
            Round.Restart += RoundRestart;
            Server.SendingRA += FixRaBc;
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
            RoundSummary.RoundLock = false;
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
        private static void FixRaBc(SendingRAEvent ev)
        {
            if ((ev.Name == "bc" || ev.Name == "broadcast") && PermissionsHandler.IsPermitted(ev.CommandSender.Permissions, PlayerPermissions.Broadcasting))
            {
                ev.Allowed = false;
                ushort num8;
                if (!ushort.TryParse(ev.Args[0], out num8) || num8 < 1)
                {
                    ev.CommandSender.RaReply(ev.Name.ToUpper() + "#First argument must be a positive integer.", false, true, "");
                    return;
                }
                string text16 = ev.Command.Substring(ev.Name.Length + ev.Args[0].Length + 2);
                API.Map.Broadcast(text16, System.Convert.ToUInt16(ev.Args[0])); ev.Success = true;
                ev.CommandSender.RaReply(ev.Name.ToUpper() + "#Broadcast sent.", false, true, "");
            }
        }
    }
}