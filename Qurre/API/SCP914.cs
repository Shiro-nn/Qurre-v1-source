using Mirror;
using Scp914;
using System.Collections.Generic;
using UnityEngine;
using Utils.ConfigHandler;
namespace Qurre.API
{
	public class Scp914
	{
		public static bool IsWorking => Scp914Machine.singleton.working;
		public static Scp914Knob KnobState
        {
			get => Scp914Machine.singleton.knobState;
			set => Scp914Machine.singleton.knobState = value;
		}
		public static ConfigEntry<Scp914Mode> Cfg
        {
			get => Scp914Machine.singleton.configMode;
			set => Scp914Machine.singleton.configMode = value;
		}
		public static Transform Intake => Scp914Machine.singleton.intake;
		public static Transform Output => Scp914Machine.singleton.output;
		public static void Activate() => Scp914Machine.singleton.RpcActivate(NetworkTime.time);
		public static void Activate(float time) => Scp914Machine.singleton.RpcActivate(time);
		public static Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> Recipes() => Scp914Machine.singleton.recipesDict;
		public static void Recipes(Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> recipes) => Scp914Machine.singleton.recipesDict = recipes;
	}
}