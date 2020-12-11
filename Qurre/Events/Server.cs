using Qurre.API.Events;
using static Qurre.API.Events.Report;
using Qurre.Events.modules;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class Server
    {
        public static event AllEvents<SendingRaEvent> SendingRA;
        public static event AllEvents<SendingConsoleEvent> SendingConsole;
        public static void sendingra(SendingRaEvent ev) => SendingRA.invoke(ev);
        public static void sendingconsole(SendingConsoleEvent ev) => SendingConsole?.Invoke(ev);
        public static class Report
        {
            public static event AllEvents<CheaterEvent> Cheater;
            public static event AllEvents<LocalEvent> Local;
            public static void cheater(CheaterEvent ev) => Cheater.Invoke(ev);
            public static void local(LocalEvent ev) => Local.Invoke(ev);
        }
    }
}