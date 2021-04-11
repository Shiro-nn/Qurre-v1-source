#pragma warning disable SA1313
using HarmonyLib;
using Respawning;
using Respawning.NamingRules;
namespace Qurre.Patches.etc
{
	[HarmonyPatch(typeof(NineTailedFoxNamingRule), nameof(NineTailedFoxNamingRule.GenerateNew))]
	internal class UnitNamePatch
	{
		internal static bool Prefix(SpawnableTeamType type) => type == SpawnableTeamType.NineTailedFox;
	}
}