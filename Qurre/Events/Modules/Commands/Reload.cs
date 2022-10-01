using System;
using System.Collections.Generic;
using CommandSystem;
namespace Qurre.Events.Modules.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class Reload : ParentCommand
    {
        internal static bool CheckPerms(string UserId)
        {
            if (UserId == "SERVER CONSOLE") return true;
            string[] str = Loader.ReloadAccess;
            List<string> strl = new();
            foreach (string st in str) strl.Add(st.Trim());
            if (strl.Contains(UserId)) return true;
            var __ = API.Player.Get(UserId);
            if (__ == null) return false;
            return strl.Contains(__.UserId) || strl.Contains(__.GroupName);
        }
        public static Reload Instance { get; } = new();
        public Reload() => LoadGeneratedCommands();
        public override string Command => "reload";
        public override string[] Aliases => new string[0];
        public override string Description => "Reloads Configs & Plugins";
        public override void LoadGeneratedCommands()
        {
            RegisterCommand(Plugins.Instance);
            RegisterCommand(Configs.Instance);
        }
        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!CheckPerms((sender as CommandSender).SenderId))
            {
                response = "Access denied";
                return false;
            }
            response = "Specify an existing subcommand.\nExamples:\n - reload plugins\n - reload configs";
            return true;
        }
    }
}