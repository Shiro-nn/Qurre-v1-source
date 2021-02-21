using System;
using System.Reflection;
namespace Qurre.API.Events
{
    public class SendingRAEvent : EventArgs
    {
        private string returnMessage;
        public SendingRAEvent(CommandSender commandSender, Player player, string command, string prefix = "", bool isAllowed = true)
        {
            CommandSender = commandSender;
            Player = player;
            Command = command;
            IsAllowed = isAllowed;
            if (prefix == "")
                pref = Assembly.GetCallingAssembly().GetName().Name;
            else
                pref = prefix;
        }
        public CommandSender CommandSender { get; }
        public Player Player { get; }
        public string Command { get; }
        public string pref;
        public string ReplyMessage
        {
            get => returnMessage;
            set => returnMessage = $"{pref}#{value}";
        }
        public bool Success { get; set; } = true;
        public bool IsAllowed { get; set; }
    }
    public class SendingConsoleEvent : EventArgs
    {
        public SendingConsoleEvent(
            Player player,
            string message,
            bool isEncrypted,
            string returnMessage = "",
            string color = "white",
            bool isAllowed = true)
        {
            Player = player;
            Message = message;
            IsEncrypted = isEncrypted;
            ReturnMessage = returnMessage;
            Color = color;
            IsAllowed = isAllowed;
        }
        public Player Player { get; }
        public string Message { get; }
        public bool IsEncrypted { get; private set; }
        public string ReturnMessage { get; set; }
        public string Color { get; set; }
        public bool IsAllowed { get; set; }
    }
    public class Report
    {
        public class CheaterEvent : EventArgs
        {
            public CheaterEvent(Player sender, Player target, int port, string reason, bool isAllowed = true)
            {
                Sender = sender;
                Target = target;
                Port = port;
                Reason = reason;
                IsAllowed = isAllowed;
            }
            public Player Sender { get; }
            public Player Target { get; }
            public int Port { get; }
            public string Reason { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class LocalEvent : EventArgs
        {
            public LocalEvent(Player issuer, Player target, string reason, bool isAllowed = true)
            {
                Issuer = issuer;
                Target = target;
                Reason = reason;
                IsAllowed = isAllowed;
            }
            public Player Issuer { get; }
            public Player Target { get; }
            public string Reason { get; set; }
            public bool IsAllowed { get; set; }
        }
    }
}