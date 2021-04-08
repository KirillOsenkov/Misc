using System;
using System.Threading;

namespace Sleep
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: sleep [500ms]|[40s]|[3m]|[2h]|[5d]");
                return 1;
            }

            TimeSpan duration = TimeSpan.Zero;

            var prefixes = new (string prefix, Func<double, TimeSpan> function)[]
            {
                ("ms", TimeSpan.FromMilliseconds),
                ("s", TimeSpan.FromSeconds),
                ("m", TimeSpan.FromMinutes),
                ("h", TimeSpan.FromHours),
                ("d", TimeSpan.FromDays)
            };

            foreach (var arg in args)
            {
                bool foundPrefix = false;
                foreach (var prefixHandler in prefixes)
                {
                    string trimmed = arg;
                    if (trimmed.EndsWith(prefixHandler.prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        trimmed = trimmed.Substring(0, trimmed.Length - prefixHandler.prefix.Length);
                        if (!double.TryParse(trimmed, out var number) || number < 0)
                        {
                            Console.Error.WriteLine($"Invalid number: {trimmed}");
                            return 2;
                        }

                        foundPrefix = true;
                        duration += prefixHandler.function(number);
                        break;
                    }
                }

                if (foundPrefix)
                {
                    continue;
                }

                if (!double.TryParse(arg, out var s) || s < 0)
                {
                    Console.Error.WriteLine($"Invalid number of seconds: {arg}");
                    return 3;
                }

                duration += TimeSpan.FromSeconds(s);
            }

            Thread.Sleep(duration);
            return 0;
        }
    }
}
