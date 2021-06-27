using Qurre.API.Events;
using static Qurre.Events.Server;
using static Qurre.Events.Server.Report;
namespace Qurre.Events.Invoke
{
    public static class Server
    {
        public static void SendingRA(SendingRAEvent ev) => Invokes(ev);
        public static void RaRequestPlayerList(RaRequestPlayerListEvent ev) => Invokes(ev);
        public static void SendingConsole(SendingConsoleEvent ev) => Invokes(ev);
        public static class Report
        {
            public static void Cheater(ReportCheaterEvent ev) => Invokes(ev);
            public static void Local(ReportLocalEvent ev) => Invokes(ev);
        }
    }
}