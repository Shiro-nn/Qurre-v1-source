using System;
namespace Patcher
{
    public class ConsoleLogger : ILogger
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void WriteLine(params string[] list)
        {
            var str = list[0];
            var args = new string[list.Length - 1];
            for (var i = 1; i < list.Length; i++)
            {
                args[i - 1] = list[i];
            }

            Console.WriteLine(str, args);

        }

        public void Write(string message)
        {
            Console.Write(message);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}