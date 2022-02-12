using Qurre.API;
using CommandSystem;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Reflection;
using Version = System.Version;
using Qurre.API.Addons;
namespace Qurre
{
	public abstract class Plugin
	{
		public static Config Config { get; set; }
		public ListConfigs CustomConfigs { get; } = new();
		public virtual Version Version { get; } = new(1, 0, 0);
		public virtual Version NeededQurreVersion { get; } = new(1, 0, 0);
		///<summary>
		///<para>Plugin load priority.</para>
		///<para>The higher the number, the earlier the plugin will load.</para>
		///</summary> 
		public QurrePluginInfo Info
                {
			get;
			internal set; 
                }

		public string Author => Info?.Author;
		public string Name => Info?.Name;
		public string Description => Info?.Name; 
		 
		public Plugin()
                { 
			if(Info != null)
                        {
				Version = new Version(Info.Version);
				NeededQurreVersion = new Version(Info.NeededVersion);
                        }
                }
		public virtual int Priority { get; } = 0;
		public abstract void Enable();
		public abstract void Disable();
		public virtual void Reload() => Log.Debug($"Reloaded.\nPlugin - {Info?.Name}\nDeveloper - {Info?.Author}\nVersion - {Info?.Version}\nNeeded Qurre Version - {Info?.NeededVersion}");
		internal Assembly Assembly;
		internal Dictionary<Type, Dictionary<Type, ICommand>> Commands = new()
		{
			{ typeof(GameConsoleCommandHandler), new Dictionary<Type, ICommand>() },
			{ typeof(ClientCommandHandler), new Dictionary<Type, ICommand>() },
			{ typeof(RemoteAdminCommandHandler), new Dictionary<Type, ICommand>() }
		};
		public virtual void RegisterCommands()
		{
			foreach (var type in Assembly.GetTypes())
			{
				if (type.GetInterface("ICommand") != typeof(ICommand) || !Attribute.IsDefined(type, typeof(CommandHandlerAttribute)))
					continue;

				foreach (CustomAttributeData customAttributeData in type.CustomAttributes)
				{
					try
					{
						if (customAttributeData.AttributeType != typeof(CommandHandlerAttribute))
							continue;

						var commandType = (Type)customAttributeData.ConstructorArguments?[0].Value;

						if (!Commands.TryGetValue(commandType, out var typeCmds))
							continue;

						if (!typeCmds.TryGetValue(type, out ICommand cmd))
							cmd = (ICommand)Activator.CreateInstance(type);

						if (commandType == typeof(RemoteAdminCommandHandler))
							CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(cmd);
						else if (commandType == typeof(GameConsoleCommandHandler))
							QueryProcessor.DotCommandHandler.RegisterCommand(cmd);
						else if (commandType == typeof(ClientCommandHandler))
							GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(cmd);

						Commands[commandType][type] = cmd;
					}
					catch (Exception ex)
					{
						Log.Error($"An error occurred while processing {type.FullName}\n{ex}");
					}
				}
			}
		}
		public virtual void UnregisterCommands()
		{
			foreach (var types in Commands)
			{
				foreach (ICommand cmd in types.Value.Values)
				{
					if (types.Key == typeof(RemoteAdminCommandHandler))
						CommandProcessor.RemoteAdminCommandHandler.UnregisterCommand(cmd);
					else if (types.Key == typeof(GameConsoleCommandHandler))
						GameCore.Console.singleton.ConsoleCommandHandler.UnregisterCommand(cmd);
					else if (types.Key == typeof(ClientCommandHandler))
						QueryProcessor.DotCommandHandler.UnregisterCommand(cmd);
				}
			}
		}
	}
}
