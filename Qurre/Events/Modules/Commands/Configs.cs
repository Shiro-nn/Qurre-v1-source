using System;
using CommandSystem;
namespace Qurre.Events.Modules.Commands
{
    internal class Configs : ICommand
    {
        public static Configs Instance { get; } = new();
        public string Command { get; } = "configs";
        public string[] Aliases { get; } = new string[0];
        public string Description { get; } = "Reload configs";
        public bool Execute(ArraySegment<string> args, ICommandSender sender, out string response)
        {
            string senderId = (sender as CommandSender).SenderId;
            if (!Reload.CheckPerms(senderId))
            {
                response = "Access denied";
                return false;
            }
            Plugin.Config.Reload();
            response = "Configs reloaded";
            return true;
        }
    }
}