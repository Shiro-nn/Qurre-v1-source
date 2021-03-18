using Mirror;
using Scp914;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.ConfigHandler;
namespace Qurre.API.Controllers
{
	public class Scp914
	{
		internal Scp914() { }
		public GameObject GameObject => Scp914Machine.singleton.gameObject;
		public bool Working => Scp914Machine.singleton.working;
		public Scp914Knob KnobState
        {
			get => Scp914Machine.singleton.knobState;
			set => Scp914Machine.singleton.knobState = value;
		}
		public ConfigEntry<Scp914Mode> Cfg
        {
			get => Scp914Machine.singleton.configMode;
			set => Scp914Machine.singleton.configMode = value;
		}
		public Transform Intake
		{
			get => Scp914Machine.singleton.intake;
			set => Scp914Machine.singleton.intake = value;
		}

		public Transform Output
		{
			get => Scp914Machine.singleton.output;
			set => Scp914Machine.singleton.output = value;
		}
		public List<Recipe> RecipesList { get; } = new List<Recipe>();
		public void Activate() => Scp914Machine.singleton.RpcActivate(NetworkTime.time);
		public void Activate(float time) => Scp914Machine.singleton.RpcActivate(time);
		public Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> Recipes() => Scp914Machine.singleton.recipesDict;
		public void Recipes(Dictionary<ItemType, Dictionary<Scp914Knob, ItemType[]>> recipes) => Scp914Machine.singleton.recipesDict = recipes;
		public int UpgradeItemID(int id)
		{
			var recipe = RecipesList.FirstOrDefault(x => x.itemID == id);
			if (recipe == null) return -1;
			List<int> ids;
			switch (KnobState)
			{
				case Scp914Knob.Rough: ids = recipe.rough; break;
				case Scp914Knob.Coarse: ids = recipe.coarse; break;
				case Scp914Knob.OneToOne: ids = recipe.oneToOne; break;
				case Scp914Knob.Fine: ids = recipe.fine; break;
				case Scp914Knob.VeryFine: ids = recipe.veryFine; break;
				default: Log.Error("Qurre.API.Controllers.Scp914.UpgradeItemID"); return -1;
			}
			return ids.Count == 0 ? -1 : ids[UnityEngine.Random.Range(0, ids.Count)];
		}
		public class Recipe
		{
			public Recipe(Scp914Recipe recipe)
			{
				itemID = (int)recipe.itemID;

				rough = recipe.rough.Select(x => (int)x).ToList();
				coarse = recipe.coarse.Select(x => (int)x).ToList();
				oneToOne = recipe.oneToOne.Select(x => (int)x).ToList();
				fine = recipe.fine.Select(x => (int)x).ToList();
				veryFine = recipe.veryFine.Select(x => (int)x).ToList();
			}
			public Recipe()
			{
				itemID = -1;
				rough = new List<int> { };
				coarse = new List<int> { };
				oneToOne = new List<int> { };
				fine = new List<int> { };
				veryFine = new List<int> { };
			}
			public int itemID;
			public List<int> rough;
			public List<int> coarse;
			public List<int> oneToOne;
			public List<int> fine;
			public List<int> veryFine;
		}
	}
}