using Mirror;
using Scp914;
using System.Collections.Generic;
using UnityEngine;
using Utils.ConfigHandler;
namespace Qurre.API
{
	public class SCP914
	{
		public static void KnobState(Scp914Knob scp914Knob) => Scp914Machine.singleton.knobState = scp914Knob;
		public static Scp914Knob KnobState() => Scp914Machine.singleton.knobState;
		public static void Start() => Scp914Machine.singleton.RpcActivate(NetworkTime.time);
		public static bool IsWorking => Scp914Machine.singleton.working;
		public static Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> Recipes() => Scp914Machine.singleton.recipesDict;
		public static void Recipes(Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> recipes) => Scp914Machine.singleton.recipesDict = recipes;
		public static ConfigEntry<Scp914Mode> Cfg() => Scp914Machine.singleton.configMode;
		public static void Cfg(ConfigEntry<Scp914Mode> config) => Scp914Machine.singleton.configMode = config;
		public static Transform Intake() => Scp914Machine.singleton.intake;
		public static Transform Output() => Scp914Machine.singleton.output;
	}
}