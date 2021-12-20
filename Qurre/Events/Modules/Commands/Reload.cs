using System;
using System.Collections.Generic;
using CommandSystem;
namespace Qurre.Events.Modules.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class Reload : ParentCommand
    {
        internal static bool CheckPerms(API.Player __)
        {
            string _ = Loader.ReloadAccess;
            string[] str = _.Split(',');
            List<string> strl = new();
            foreach (string st in str) strl.Add(st.Trim());
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
            string senderId = (sender as CommandSender).SenderId;
            if (!CheckPerms(API.Player.Get(senderId)) && senderId != "SERVER CONSOLE")
            {
                response = "Access denied";
                return false;
            }
            response = "Specify an existing subcommand.\nExamples:\n - reload plugins\n - reload configs";
            return true;
        }
    }
}