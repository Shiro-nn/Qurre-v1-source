using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.Main;
namespace Qurre.Events
{
    public static class Alpha
    {
        public static event AllEvents<AlphaStopEvent> Stopping;
        public static event AllEvents<AlphaStartEvent> Starting;
        public static event AllEvents Detonated;
        public static event AllEvents<EnableAlphaPanelEvent> EnablePanel;
        internal static void Invokes(AlphaStopEvent ev) => Stopping?.invoke(ev);
        internal static void Invokes(AlphaStartEvent ev) => Starting?.invoke(ev);
        internal static void Invokes() => Detonated?.invoke();
        internal static void Invokes(EnableAlphaPanelEvent ev) => EnablePanel?.invoke(ev);
    }
}