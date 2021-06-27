using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.Main;
namespace Qurre.Events
{
    public static class Server
    {
        public static event AllEvents<SendingRAEvent> SendingRA;
        public static event AllEvents<RaRequestPlayerListEvent> RaRequestPlayerList;
        public static event AllEvents<SendingConsoleEvent> SendingConsole;
        internal static void Invokes(SendingRAEvent ev) => SendingRA.invoke(ev);
        internal static void Invokes(RaRequestPlayerListEvent ev) => RaRequestPlayerList.invoke(ev);
        internal static void Invokes(SendingConsoleEvent ev) => SendingConsole?.invoke(ev);
        public static class Report
        {
            public static event AllEvents<ReportCheaterEvent> Cheater;
            public static event AllEvents<ReportLocalEvent> Local;
            internal static void Invokes(ReportCheaterEvent ev) => Cheater?.invoke(ev);
            internal static void Invokes(ReportLocalEvent ev) => Local?.invoke(ev);
        }
    }
}