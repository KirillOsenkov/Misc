using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        var sb = new StringBuilder();
        foreach (var item in GetSequence().Take(50))
        {
            sb.Append(item.ToString());
            sb.Append(" ");
        }

        Console.WriteLine(sb.ToString());
        Console.ReadKey();
    }

    private static IEnumerable<int> GetSequence()
    {
        var queue = new Queue<int>();
        int current = 1;
        while (true)
        {
            yield return current;
            queue.Enqueue(current);

            var currentRun = queue.Dequeue();
            if (currentRun == 2)
            {
                yield return current;
                queue.Enqueue(current);
            }

            current = 3 - current;
        }
    }
}