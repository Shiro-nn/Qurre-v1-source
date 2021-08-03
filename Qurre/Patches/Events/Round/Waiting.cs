using HarmonyLib;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
    internal static class Waiting
    {
        private static void Prefix(string q)
        {
            if (q == "Waiting for players...")
            {
                API.Map.AddObjects();
                API.Round.CurrentRound++;
                Qurre.Events.Invoke.Round.WaitingForPlayers();
            }
        }
    }
}