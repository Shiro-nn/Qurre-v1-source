using System;
using System.Reflection;
namespace Qurre.API.Events
{
    public class SendingRaEvent : EventArgs
    {
        private string returnMessage;
        public SendingRaEvent(CommandSender commandSender, ReferenceHub sender, string message, string prefix = "", bool isAllowed = true)
        {
            CommandSender = commandSender;
            Sender = sender;
            Message = message;
            IsAllowed = isAllowed;
            if (prefix == "")
                Pref = Assembly.GetCallingAssembly().GetName().Name;
            else
                Pref = prefix;
        }
        public CommandSender CommandSender { get; }
        public ReferenceHub Sender { get; }
        public string Message { get; }
        public string Pref;
        public string ReplyMessage
        {
            get => returnMessage;
            set => returnMessage = $"{Pref}#{value}";
        }
        public bool Success { get; set; } = true;
        public bool IsAllowed { get; set; }
    }
    public class SendingConsoleEvent : EventArgs
    {
        public SendingConsoleEvent(
            ReferenceHub player,
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
        public ReferenceHub Player { get; }
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
            public CheaterEvent(ReferenceHub sender, ReferenceHub target, int port, string reason, bool isAllowed = true)
            {
                Sender = sender;
                Target = target;
                Port = port;
                Reason = reason;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Sender { get; }
            public ReferenceHub Target { get; }
            public int Port { get; }
            public string Reason { get; set; }
            public bool IsAllowed { get; set; }
        }
        public class LocalEvent : EventArgs
        {
            public LocalEvent(ReferenceHub issuer, ReferenceHub target, string reason, bool isAllowed = true)
            {
                Issuer = issuer;
                Target = target;
                Reason = reason;
                IsAllowed = isAllowed;
            }
            public ReferenceHub Issuer { get; }
            public ReferenceHub Target { get; }
            public string Reason { get; set; }
            public bool IsAllowed { get; set; }
        }
    }
}