using CommandSystem;
using System;

namespace Qurre.Events.Modules.Commands
{
    internal class Plugins : ICommand
    {
        public static Plugins Instance { get; } = new();

        public string Command { get; } = "plugins";

        public string[] Aliases { get; } = new string[0];

        public string Description { get; } = "Reload plugins";

        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            if (!Reload.CheckPerms((sender as CommandSender).SenderId))
            {
                response = "Access denied";
                return false;
            }

            PluginManager.ReloadPlugins();

            response = "Plugins reloaded";
            return true;
        }
    }
}