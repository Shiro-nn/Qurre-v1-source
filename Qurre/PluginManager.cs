using HarmonyLib;
using MEC;
using QurreModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace Qurre
{
	public class PluginManager
	{
<<<<<<< HEAD
		public static readonly List<Plugin> plugins = new List<Plugin>();
		public const string Version = "1.0.1";
=======
		public static readonly List<Plugin> Plugins = new List<Plugin>();
		public const string Version = "1.0.0";
>>>>>>> 5ad90713bfe2d60644606aff8380b4ef8b0a4280
		public static string Plan { get; private set; } = "Lite";
		public static int Planid { get; private set; } = 1;
		private static string domen = "localhost"; //qurre.store
		public static string AppDataDirectory { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public static string QurreDirectory { get; private set; } = Path.Combine(AppDataDirectory, "Qurre");
		public static string PluginsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Plugins");
		public static string LoadedDependenciesDirectory { get; private set; } = Path.Combine(PluginsDirectory, "dependencies");
		public static string ConfigsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Configs");
		public static string ManagedAssembliesDirectory { get; private set; } = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");
		public static string ConfigsPath { get; internal set; }
		private static Harmony hInstance;
		public static IEnumerator<float> LoadPlugins()
		{
			if (!Directory.Exists(PluginsDirectory))
			{
				Log.Warn($"plugins directory not found - creating: {PluginsDirectory}");
				Directory.CreateDirectory(PluginsDirectory);
			}
			try
			{
				LoadDependencies();
			}
			catch (Exception exception)
			{
				ServerConsole.AddLog(exception.ToString(), ConsoleColor.Red);
			}
			//CheckPlan();

			PatchMethods();

			yield return Timing.WaitForSeconds(0.5f);

			List<string> mods = Directory.GetFiles(PluginsDirectory).ToList();

			foreach (string mod in mods)
			{
				if (mod.EndsWith("Qurre.dll"))
					continue;

				LoadPlugin(mod);
			}

			Enable();
		}
		private static void LoadDependencies()
		{
			Log.Custom("Loading dependencies...", "Loader", ConsoleColor.Magenta);

			if (!Directory.Exists(LoadedDependenciesDirectory))
				Directory.CreateDirectory(LoadedDependenciesDirectory);

			string[] depends = Directory.GetFiles(LoadedDependenciesDirectory);

			foreach (string dll in depends)
			{
				if (!dll.EndsWith(".dll"))
					continue;

				if (IsLoaded(dll))
					return;

				if (dll.EndsWith("0Harmony.dll") || dll.EndsWith("YamlDotNet.dll"))
					continue;

				Assembly assembly = Assembly.LoadFrom(dll);
				localLoaded.Add(assembly);
				Log.Custom("Loaded dependency " + assembly.FullName, "Loader", ConsoleColor.Blue);
			}
			Log.Custom("Dependencies loaded!", "Loader", ConsoleColor.Green);
		}
		private static bool IsLoaded(string a)
		{
			foreach (Assembly asm in localLoaded)
			{
				if (asm.Location == a)
					return true;
			}

			return false;
		}
		private static List<Assembly> localLoaded = new List<Assembly>();
		public static void LoadPlugin(string mod)
		{
			Log.Info($"Loading {mod}");
			try
			{
				byte[] file = ModLoader.ReadFile(mod);
				Assembly assembly = Assembly.Load(file);

				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsAbstract)
					{
						Log.Debug($"{type.FullName} is abstract, skipping.");
						continue;
					}

					if (!typeof(Plugin).IsAssignableFrom(type))
					{
						Log.Debug($"{type.FullName} doesn't inherit from Qurre.Plugin, skipping.");
						continue;
					}

					Log.Info($"Loading type {type.FullName}");
					object plugin = Activator.CreateInstance(type);
					Log.Info($"Instantiated type {type.FullName}");

					if (!(plugin is Plugin p))
					{
						Log.Error($"{type.FullName} not a plugin");
						continue;
					}

					Plugins.Add(p);
					Log.Info($"{type.FullName} loaded");
				}
			}
			catch (Exception exception)
			{
				Log.Error($"Error while initalizing {mod}!\n{exception}");
			}
		}
		public static void Enable()
		{
			foreach (Plugin plugin in Plugins)
			{
				try
				{
					plugin.Enable();
				}
				catch (Exception exception)
				{
					Log.Error($"Plugin {plugin.name} threw an exception while enabling\n{exception}");
				}
			}
		}
		public static void Reload()
		{
			foreach (Plugin plugin in Plugins)
			{
				try
				{
					plugin.Reload();
				}
				catch (Exception exception)
				{
					Log.Error($"Plugin {plugin.name} threw an exception while reloading\n{exception}");
				}
			}
		}
		public static void Disable()
		{
			foreach (Plugin plugin in Plugins)
			{
				try
				{
					plugin.Disable();
				}
				catch (Exception exception)
				{
					Log.Error($"Plugin {plugin.name} threw an exception while disabling\n{exception}");
				}
			}
		}
		public static void ReloadPlugins()
		{
			try
			{
				Log.Info($"Reloading Plugins...");
				Disable();
				Reload();
				Plugins.Clear();
				UnPatchMethods();

				Timing.RunCoroutine(LoadPlugins());
			}
			catch (Exception exception)
			{
				Log.Error($"umm, error in reloading.\n{exception}");
			}
		}
		private static void PatchMethods()
		{
			try
			{
				hInstance = new Harmony("qurre.patches");
				hInstance.PatchAll();
				Log.Info("Harmony successfully Patching");
			}
			catch (Exception e)
			{
				Log.Error($"Harmony Patching threw an error:\n{e}");
			}
		}
		private static void UnPatchMethods()
		{
			try
			{
				hInstance.UnpatchAll(null);
				Log.Info("Harmony successfully UnPatching");
			}
			catch (Exception e)
			{
				Log.Error($"Harmony UnPatching threw an error:\n{e}");
			}
		}
		private static void CheckPlan()
		{
			try
			{
				var url = $"http://{domen}/check";
				var req = System.Net.WebRequest.Create(url);
				var resp = req.GetResponse();
				using (var sr = new System.IO.StreamReader(resp.GetResponseStream()))
				{
					var response = sr.ReadToEnd().Trim();
					var a = response.Split(':')[1].Substring(1).Split('<')[0];
					if (a == "2")
					{
						Planid = 2;
						Plan = "Premium";
						Log.Custom("Qurre plan: Premium", "Plan", ConsoleColor.Green);
					}
					else if (a == "3")
					{
						Planid = 3;
						Plan = "Gold";
						Log.Custom("Qurre plan: Gold", "Plan", ConsoleColor.Yellow);
					}
					else if (a == "4")
					{
						Planid = 4;
						Plan = "Platinum";
						Log.Custom("Qurre plan: Platinum", "Plan", ConsoleColor.White);
					}
					else
					{
						Log.Custom("Qurre plan: Lite", "Plan", ConsoleColor.Magenta);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error($"Plan checking threw an error:\n{e}");
			}
		}
	}
}
