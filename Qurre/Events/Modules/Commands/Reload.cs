using System;
using System.Collections.Generic;
using CommandSystem;
namespace Qurre.Events.Modules.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class Reload : ParentCommand
    {
        internal static bool CheckPerms(string senderId)
        {
            try
            {
                if (senderId == "SERVER CONSOLE") return true;
                string _ = Loader.ReloadAccess;
                string[] str = _.Split(',');
                List<string> strl = new();
                foreach (string st in str) strl.Add(st.Trim());
                if (strl.Contains(senderId)) return true;
                var __ = API.Player.Get(senderId);
                if (__ == null) return false;
                return strl.Contains(__.UserId) || strl.Contains(__.GroupName);
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while processing {senderId}\n{ex}");
                return false;
            }
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
            if (!CheckPerms(senderId))
            {
                response = "Access denied";
                return false;
            }
            response = "Specify an existing subcommand.\nExamples:\n - reload plugins\n - reload configs";
            return true;
        }
    }
}