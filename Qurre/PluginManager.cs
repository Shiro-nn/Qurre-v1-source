﻿using HarmonyLib;
using MEC;
using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
namespace Qurre
{
	public static class PluginManager
	{
		internal static readonly List<Plugin> _plugins = new();
		internal static Harmony _harmony;

		//private static string Domain { get; } = "localhost"; //qurre.team
		public static Version Version { get; } = new Version(1, 15, 0);
		public static string AppDataDirectory { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		public static string QurreDirectory { get; private set; } = Path.Combine(AppDataDirectory, "Qurre");
		public static string PluginsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Plugins");
		public static string LoadedDependenciesDirectory { get; private set; } = Path.Combine(PluginsDirectory, "dependencies");
		public static string ConfigsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Configs");
		public static string CustomConfigsDirectory { get; private set; } = Path.Combine(ConfigsDirectory, "Custom");
		public static string LogsDirectory { get; private set; } = Path.Combine(QurreDirectory, "Logs");
		public static string ManagedAssembliesDirectory { get; private set; } = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");
        [Obsolete("Outdated; Use 'JsonConfig'")]
		public static string ConfigsPath { get; internal set; }
		internal static IEnumerator<float> LoadPlugins()
		{
			//Harmony.DEBUG = true;
			if (!Directory.Exists(PluginsDirectory))
			{
				Log.Warn($"Plugins directory not found - creating: {PluginsDirectory}");
				Directory.CreateDirectory(PluginsDirectory);
			}

			try { LoadDependencies(); }
			catch (Exception ex) { ServerConsole.AddLog(ex.ToString(), ConsoleColor.Red); }

			PatchMethods();

			yield return Timing.WaitForSeconds(0.5f);

			foreach (string plugin in Directory.GetFiles(PluginsDirectory))
			{
				try
				{
					Log.Debug($"Loading {plugin}");
					LoadPlugin(Assembly.Load(LoaderManager.ReadFile(plugin)));
				}
				catch (Exception ex)
				{
					Log.Error($"An error occurred while loading {plugin}\n{ex}");
				}
			}
			DownloadPlugins();

			Enable();
		}
		private static void LoadDependencies()
		{
			if (!Directory.Exists(LoadedDependenciesDirectory))
				Directory.CreateDirectory(LoadedDependenciesDirectory);
			foreach (string dll in Directory.GetFiles(LoadedDependenciesDirectory))
			{
				if (!dll.EndsWith(".dll") || LoaderManager.Loaded(dll)) continue;
				Assembly assembly = Assembly.Load(LoaderManager.ReadFile(dll));
				LoaderManager.LocalLoaded.Add(new(assembly, dll));
				Log.Custom("Loaded dependency " + assembly.FullName, "Loader", ConsoleColor.Blue);
			}
			DownloadDependencies();
			Log.Custom("Dependencies loaded!", "Loader", ConsoleColor.Green);
		}
		public static void LoadPlugin(Assembly assembly)
		{
			try
			{
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

					if (plugin is not Plugin p)
					{
						Log.Error($"{type.FullName} not a plugin");
						continue;
					}

					if (!CheckPlugin(p)) continue;
					p.Assembly = assembly;

					_plugins.Add(p);
					Log.Debug($"{type.FullName} loaded");
				}
			}
			catch (Exception ex)
			{
				Log.Error($"An error occurred while processing {assembly.FullName}\n{ex}");
			}
		}
		public static bool CheckPlugin(Plugin plugin)
		{
			if (Version.Major != plugin.NeededQurreVersion.Major)
			{
				if (Version.Major > plugin.NeededQurreVersion.Major)
				{
					Log.Warn($"Plugin {plugin.Name} not loaded because he is outdated. Qurre Version: {Version.ToString(3)}. " +
                        $"Needed Version: {plugin.NeededQurreVersion}.");
					return false;
				}

				if (Version.Major < plugin.NeededQurreVersion.Major)
				{
					Log.Warn($"Plugin {plugin.Name} not loaded because your Qurre version is outdated. Qurre Version: {Version.ToString(3)}. " +
                        $"Needed Version: {plugin.NeededQurreVersion}.");
					return false;
				}
			}
			else if (Version < plugin.NeededQurreVersion)
			{
				Log.Warn($"Plugin {plugin.Name} not loaded. Requires Qurre version at least {plugin.NeededQurreVersion}, your version: {Version}");
				return false;
			}

			return true;
		}
		public static void Enable()
		{
			foreach (Plugin plugin in _plugins.OrderByDescending(o => o.Priority))
			{
				try
				{
					plugin.Enable();
					plugin.RegisterCommands();
					Log.Info($"Plugin {plugin.Name} written by {plugin.Developer} enabled. v{plugin.Version}");
				}
				catch (Exception ex)
				{
					Log.Error($"Plugin {plugin.Name} threw an exception while enabling\n{ex}");
				}
			}
		}
		public static void InvokeReload()
		{
			foreach (Plugin plugin in _plugins)
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
			foreach (Plugin plugin in _plugins)
			{
				try
				{
					plugin.Disable();
					plugin.UnregisterCommands();
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
				Log.Info("Reloading Plugins...");
				Disable();
				InvokeReload();
				_plugins.Clear();
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
				_harmony = new Harmony("qurre.patches");
				_harmony.PatchAll();
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
				_harmony.UnpatchAll(_harmony.Id);
				Log.Info("Harmony successfully UnPatched");
			}
			catch (Exception ex)
			{
				Log.Error($"Harmony UnPatching threw an error:\n{ex}");
			}
		}
		private static Assembly LoadFromUrl(string link) // Web Loader
		{
			try
			{
				WebClient _web = new();
				Assembly assembly = Assembly.Load(_web.DownloadData(link));
				return assembly;
			}
			catch (Exception e)
			{
				Log.Error($"{e}");
				return null;
			}
		}
		private static void DownloadDependencies()
		{/*
			try
			{
				var url = $"http://{Domain}/dependencies";
				var req = WebRequest.Create(url);
				var resp = req.GetResponse();
				using var sr = new StreamReader(resp.GetResponseStream());
				var response = sr.ReadToEnd().Trim();
				foreach (string str in response.Split('\n'))
				{
					Assembly assembly = LoadFromUrl(str);
					global::Loader.LocalLoaded.Add(assembly);
					Log.Custom("Loaded dependency " + assembly.FullName, "Loader", ConsoleColor.Blue);
				}
			}
			catch (Exception ex)
			{
				Log.Error($"Plan checking threw an error:\n{ex}");
			}*/
		}
		private static void DownloadPlugins()
		{/*
			try
			{
				var url = $"http://{Domain}/plugins";
				var req = WebRequest.Create(url);
				var resp = req.GetResponse();
                using var sr = new StreamReader(resp.GetResponseStream());
                var response = sr.ReadToEnd().Trim();
                foreach (string str in response.Split('\n'))
                {
                    Assembly assembly = LoadFromUrl(str);
					LoadPlugin(assembly);
                }
            }
			catch (Exception ex)
			{
				Log.Error($"Plan checking threw an error:\n{ex}");
			}*/
		}
	}
}