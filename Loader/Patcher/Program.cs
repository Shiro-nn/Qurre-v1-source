using System;
namespace Patcher
{
    internal class Program
    {
        public static void Main(string[] _)
        {
            Injector inj = new(new ConsoleLogger());
            inj.Init();
            Console.ReadKey();
        }
    }
}