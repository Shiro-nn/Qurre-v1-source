namespace Qurre.Events.modules
{
    public static class main
    {
        public delegate void AllEvents<TEventArgs>(TEventArgs ev)
            where TEventArgs : System.EventArgs;
        public delegate void AllEvents();
    }
}