namespace Qurre.Events.modules
{
    public static class Main
    {
        public delegate void AllEvents<TEventArgs>(TEventArgs ev)
            where TEventArgs : System.EventArgs;
        public delegate void AllEvents();
    }
}