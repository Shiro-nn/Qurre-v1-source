using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class Server
    {
        public static event AllEvents<SendingRAEvent> SendingRA;
        public static event AllEvents<RaRequestPlayerListEvent> RaRequestPlayerList;
        public static event AllEvents<SendingConsoleEvent> SendingConsole;
        public static void sendingra(SendingRAEvent ev) => SendingRA.invoke(ev);
        public static void raRequestPlayerList(RaRequestPlayerListEvent ev) => RaRequestPlayerList.invoke(ev);
        public static void sendingconsole(SendingConsoleEvent ev) => SendingConsole?.Invoke(ev);
        public static class Report
        {
            public static event AllEvents<ReportCheaterEvent> Cheater;
            public static event AllEvents<ReportLocalEvent> Local;
            public static void cheater(ReportCheaterEvent ev) => Cheater?.Invoke(ev);
            public static void local(ReportLocalEvent ev) => Local?.Invoke(ev);
        }
    }
}