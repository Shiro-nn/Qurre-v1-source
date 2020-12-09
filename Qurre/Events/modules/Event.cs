using System;
namespace Qurre.Events.modules
{
    public static class Event
    {
        /// <typeparam name="T">Event arg type.</typeparam>
        /// <param name="ev">Source event.</param>
        /// <param name="arg">Event arg.</param>
        /// <exception cref="ArgumentNullException">Event or its arg is null.</exception>
        public static void invoke<T>(this main.AllEvents<T> ev, T arg)
            where T : EventArgs
        {
            if (ev == null)
                return;

            var eventName = ev.GetType().FullName;
            foreach (main.AllEvents<T> handler in ev.GetInvocationList())
            {
                try
                {
                    handler(arg);
                }
                catch (Exception ex)
                {
                    Log.Error($"umm, method '{handler.Method.Name}' of class '{handler.Method.ReflectedType.FullName}' threw an exception. Event: {eventName}\n{ex}");
                }
            }
        }
        /// <param name="ev">Source event.</param>
        /// <exception cref="ArgumentNullException">Event is null.</exception>
        public static void invoke(this main.AllEvents ev)
        {
            if (ev == null)
                return;

            string eventName = ev.GetType().FullName;
            foreach (main.AllEvents handler in ev.GetInvocationList())
            {
                try
                {
                    handler();
                }
                catch (Exception ex)
                {
                    Log.Error($"umm, method '{handler.Method.Name}' of class '{handler.Method.ReflectedType?.FullName}' threw an exception. Event: {eventName}\n{ex}");
                }
            }
        }
    }
}
