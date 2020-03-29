using System;

class Program
{
    static void Main()
    {
        int a = 0;
        int b = 1;
        for (int i = 0; i < 1000; i++)
        {
            Console.WriteLine($"{a}/{b}");
            var t = b;
            b += a - 2 * (a % b);
            a = t;
        }
    }
}

