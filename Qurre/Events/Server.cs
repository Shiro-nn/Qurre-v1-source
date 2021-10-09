using Qurre.API.Events;
using Qurre.Events.Modules;
using static Qurre.Events.Modules.Main;
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
    }
}