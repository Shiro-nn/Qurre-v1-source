using HarmonyLib;
using MEC;
using QurreModLoader;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
namespace Qurre
{
	public class PluginManager
	{
		public static readonly List<Plugin> plugins = new List<Plugin>();
		public static Version Version { get; } = new Version(1, 6, 0);
		private static string Domain { get; } = "localhost"; //qurre.team
		public static string AppDataDirectory { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public static string QurreDirectory { get; private set; } = Path.Combine(AppDataDirectory, "Qurre");
		public static string PluginsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Plugins");
		public static string LoadedDependenciesDirectory { get; private set; } = Path.Combine(PluginsDirectory, "dependencies");
		public static string ConfigsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Configs");
		public static string LogsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Logs");
		public static string ManagedAssembliesDirectory { get; private set; } = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");
		public static string ConfigsPath { get; internal set; }
		private static Harmony hInstance;
		internal static IEnumerator<float> LoadPlugins()
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
			catch (Exception ex)
			{
				ServerConsole.AddLog(ex.ToString(), ConsoleColor.Red);
			}
			//CheckPlan();

			PatchMethods();

			yield return Timing.WaitForSeconds(0.5f);

			List<string> mods = Directory.GetFiles(PluginsDirectory).ToList();

			foreach (string mod in mods)
			{
				if (mod.EndsWith("Qurre.dll")) continue;
				LoadPlugin(mod);
			}
			FixAudio();
			Enable();
		}
		private static void LoadDependencies()
		{
			if (!Directory.Exists(LoadedDependenciesDirectory))
				Directory.CreateDirectory(LoadedDependenciesDirectory);

			string[] depends = Directory.GetFiles(LoadedDependenciesDirectory);
			foreach (string dll in depends)
			{
				if (!dll.EndsWith(".dll")) continue;
				if (IsLoaded(dll)) continue;
				if (dll.EndsWith("0Harmony.dll") || dll.EndsWith("YamlDotNet.dll") || dll.EndsWith("MongoDB.Bson.dll") || dll.EndsWith("DnsClient.dll")
					 || dll.EndsWith("MongoDB.Driver.Core.dll") || dll.EndsWith("MongoDB.Driver.dll") || dll.EndsWith("MongoDB.Libmongocrypt.dll"))
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
			Log.Debug($"Loading {mod}");
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

					Log.Debug($"Loading type {type.FullName}");
					object plugin = Activator.CreateInstance(type);
					Log.Debug($"Instantiated type {type.FullName}");

					if (!(plugin is Plugin p))
					{
						Log.Error($"{type.FullName} not a plugin");
						continue;
					}

					if (Version < p.NeededQurreVersion)
					{
						Log.Warn($"Plugin {p.Name} not loaded. Requires Qurre version at least {p.NeededQurreVersion}, your version: {Version}");
						continue;
					}

					plugins.Add(p);
					Log.Debug($"{type.FullName} loaded");
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Error while initalizing {mod}!\n{ex}");
			}
		}
		public static void Enable()
		{
			foreach (Plugin plugin in plugins.OrderBy(o => o.Priority).Reverse())
			{
				try
				{
					plugin.Enable();
					Log.Info($"Plugin {plugin.Name} written by {plugin.Developer} enabled. v{plugin.Version}");
				}
				catch (Exception ex)
				{
					Log.Error($"Plugin {plugin.Name} threw an exception while enabling\n{ex}");
				}
			}
		}
		public static void Reload()
		{
			foreach (Plugin plugin in plugins)
			{
				try
				{
					plugin.Reload();
					Log.Info($"Plugin {plugin.Name} reloaded.");
				}
				catch (Exception ex)
				{
					Log.Error($"Plugin {plugin.Name} threw an exception while reloading\n{ex}");
				}
			}
		}
		public static void Disable()
		{
			foreach (Plugin plugin in plugins)
			{
				try
				{
					plugin.Disable();
					Log.Info($"Plugin {plugin.Name} disabled.");
				}
				catch (Exception ex)
				{
					Log.Error($"Plugin {plugin.Name} threw an exception while disabling\n{ex}");
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
				plugins.Clear();
				UnPatchMethods();

				Timing.RunCoroutine(LoadPlugins());
			}
			catch (Exception ex)
			{
				Log.Error($"umm, error in reloading.\n{ex}");
			}
		}
		private static void PatchMethods()
		{
			try
			{
				hInstance = new Harmony("qurre.patches");
				hInstance.PatchAll();
				Log.Info("Harmony successfully Patched");
			}
			catch (Exception ex)
			{
				Log.Error($"Harmony Patching threw an error:\n{ex}");
			}
		}
		private static void UnPatchMethods()
		{
			try
			{
				hInstance.UnpatchAll(null);
				Log.Info("Harmony successfully UnPatched");
			}
			catch (Exception ex)
			{
				Log.Error($"Harmony UnPatching threw an error:\n{ex}");
			}
		}
		private static void FixAudio()
		{
			try
			{
				var fl = Path.Combine(ManagedAssembliesDirectory, "DissonanceVoip.dll");
				if (!File.Exists(fl)) UpdateAudio();
				else
				{
					FileVersionInfo Dissonance = FileVersionInfo.GetVersionInfo(fl);
					if (!Dissonance.LegalCopyright.Contains("Qurre")) UpdateAudio();
				}
			}
			catch (Exception e)
			{
				Log.Warn($"Checking for DissonanceVoip update threw an error:\n{e}");
			}
		}
		private static void UpdateAudio()
		{
			try
			{
				var fl = Path.Combine(ManagedAssembliesDirectory, "DissonanceVoip.dll");
				if (File.Exists(fl)) File.Delete(fl);
				ServerConsole.AddLog($"[WARN] [Qurre Loader] DissonanceVoip.dll outdated. Downloading DissonanceVoip.dll", ConsoleColor.Red);
				WebRequest request = WebRequest.Create("https://cdn.scpsl.store/qurre/audio/DissonanceVoip.dll");
				WebResponse response = request.GetResponse();
				Stream responseStream = response.GetResponseStream();
				Stream fileStream = File.OpenWrite(Path.Combine(ManagedAssembliesDirectory, "DissonanceVoip.dll"));
				byte[] buffer = new byte[4096];
				int bytesRead = responseStream.Read(buffer, 0, 4096);
				while (bytesRead > 0)
				{
					fileStream.Write(buffer, 0, bytesRead);
					bytesRead = responseStream.Read(buffer, 0, 4096);
				}
			}
			catch (Exception e)
			{
				Log.Warn($"Updating DissonanceVoip threw an error:\n{e}");
			}
		}
		private static void LoadFromUrl(string link) // Web Loader
		{
			try
			{
				WebClient wc = new WebClient();
				Assembly assembly = Assembly.Load(wc.DownloadData(link));
			}
			catch (Exception e)
			{
				Log.Error($"{e}");
			}
		}
		private static void CheckPlan() // Web checker
		{
			try
			{
				var url = $"http://{Domain}/check";
				var req = WebRequest.Create(url);
				var resp = req.GetResponse();
				using (var sr = new StreamReader(resp.GetResponseStream()))
				{
					var response = sr.ReadToEnd().Trim();
					var a = response.Split(':')[1].Substring(1).Split('<')[0];
					if (a == "2")
					{
						Log.Custom("Qurre plan: Premium", "Plan", ConsoleColor.Green);
					}
					else if (a == "3")
					{
						Log.Custom("Qurre plan: Gold", "Plan", ConsoleColor.Yellow);
					}
					else if (a == "4")
					{
						Log.Custom("Qurre plan: Platinum", "Plan", ConsoleColor.White);
					}
					else
					{
						Log.Custom("Qurre plan: Lite", "Plan", ConsoleColor.Magenta);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Plan checking threw an error:\n{ex}");
			}
		}
	}
}