using Qurre.API.Events;
using UnityEngine.SceneManagement;
namespace Qurre.Events.modules
{
    internal class Etc
    {
        internal static void Load()
        {
            SceneManager.sceneUnloaded += SceneUnloaded;
            MapGeneration.SeedSynchronizer.OnMapGenerated += Map.OnGenerated;
            Round.WaitingForPlayers += WaitingForPlayers;
            Player.RoleChange += ChangeRole;
        }
        private static void SceneUnloaded(Scene _)
        {
            API.Player.IdPlayers.Clear();
            API.Player.UserIDPlayers.Clear();
            API.Player.Dictionary.Clear();
        }
        private static void WaitingForPlayers() => RoundSummary.RoundLock = false;
        private static void ChangeRole(RoleChangeEvent ev)
        {
            if (ev.Player?.IsHost != false || string.IsNullOrEmpty(ev.Player.UserId)) return;
            if (ev.NewRole == RoleType.Spectator) ev.Player.Inventory.ServerDropAll();
        }
    }
}