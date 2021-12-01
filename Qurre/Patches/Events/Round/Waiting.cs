using HarmonyLib;
namespace Qurre.Patches.Events.Round
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
    internal static class Waiting
    {
        private static void Postfix(string q)
        {
            if (q == "Waiting for players...")
            {
                API.Round.CurrentRound++;
                Qurre.Events.Invoke.Round.Waiting();
            }
        }
    }
}