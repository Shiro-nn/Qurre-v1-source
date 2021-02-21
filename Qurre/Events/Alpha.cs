using Qurre.API.Events;
using Qurre.Events.modules;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class Alpha
    {
        public static event AllEvents<AlphaStopEvent> Stopping;
        public static event AllEvents<AlphaStartEvent> Starting;
        public static event AllEvents Detonated;
        public static event AllEvents<EnableAlphaPanelEvent> EnablePanel;
        public static void stopping(AlphaStopEvent ev) => Stopping?.invoke(ev);
        public static void starting(AlphaStartEvent ev) => Starting?.invoke(ev);
        public static void detonated() => Detonated?.invoke();
        public static void enablepanel(EnableAlphaPanelEvent ev) => EnablePanel?.invoke(ev);
    }
}