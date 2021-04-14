using Qurre.API.Events;
using UnityEngine.SceneManagement;
namespace Qurre.Events.modules
{
    internal class Etc
    {
        internal static void Load()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
            MapGeneration.SeedSynchronizer.OnMapGenerated += Map.generated;
            Round.WaitingForPlayers += WaitingForPlayers;
            Player.RoleChange += ChangeRole;
            Round.Restart += RoundRestart;
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
            if(Plugin.Config.GetBool("Qurre_AllUnit", false))
            {
                API.Round.AddUnit(API.Objects.TeamUnitType.ClassD, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.ChaosInsurgency, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.NineTailedFox, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Scientist, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Scp, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
                API.Round.AddUnit(API.Objects.TeamUnitType.Tutorial, $"<color=#00ff00>Qurre v{PluginManager.Version}</color>");
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
    }
}