using Qurre.API.Events.Alpha;
using Qurre.Events.modules;
using static Qurre.Events.modules.main;
namespace Qurre.Events
{
    public static class Alpha
    {
        public static event AllEvents<StopEvent> Stopping;
        public static event AllEvents<StartEvent> Starting;
        public static event AllEvents Detonated;
        public static event AllEvents<EnablePanelEvent> EnablePanel;
        public static void stopping(StopEvent ev) => Stopping?.invoke(ev);
        public static void starting(StartEvent ev) => Starting?.invoke(ev);
        public static void detonated() => Detonated?.invoke();
        public static void enablepanel(EnablePanelEvent ev) => EnablePanel?.invoke(ev);
    }
}