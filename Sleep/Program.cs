using System;
using System.Threading;

namespace Sleep
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: sleep [500ms]|[42s]");
                return 1;
            }

            TimeSpan duration;
            var arg = args[0];
            if (arg.EndsWith("ms"))
            {
                arg = arg.Substring(0, arg.Length - 2);
                if (!int.TryParse(arg, out var ms) || ms < 0)
                {
                    Console.Error.WriteLine($"Invalid number of milliseconds: {arg}");
                    return 2;
                }

                duration = TimeSpan.FromMilliseconds(ms);
            }
            else
            {
                if (arg.EndsWith("s"))
                {
                    arg = arg.Substring(0, arg.Length - 1);
                }

                if (!int.TryParse(arg, out var s) || s < 0)
                {
                    Console.Error.WriteLine($"Invalid number of seconds: {arg}");
                    return 3;
                }

                duration = TimeSpan.FromSeconds(s);
            }

            Thread.Sleep(duration);
            return 0;
        }
    }
}
