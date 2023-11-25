using System;

class Program
{
    static void Main()
    {
        Solve(numbers: 9, pluses: 3, minuses: 1, target: 100, stringSoFar: "");
    }

    static void Solve(int numbers, int pluses, int minuses, int target, string stringSoFar = "")
    {
        if (numbers <= pluses + minuses)
        {
            return;
        }

        int currentNumber = 0;

        for (int i = numbers; i > pluses + minuses; i--)
        {
            currentNumber = currentNumber * 10 + i;

            if (pluses > 0)
            {
                string plusString = stringSoFar;

                if (plusString != "")
                {
                    plusString = plusString + " + ";
                }

                plusString = $"{plusString}{currentNumber}";

                Solve(i - 1, pluses - 1, minuses, target - currentNumber, $"{plusString}");
            }

            if (minuses > 0)
            {
                string minusString = stringSoFar;

                if (minusString != "")
                {
                    minusString = minusString + " - ";
                }
                else
                {
                    minusString = "-";
                }

                minusString = $"{minusString}{currentNumber}";

                Solve(i - 1, pluses, minuses - 1, target + currentNumber, $"{minusString}");
            }
        }

        if (currentNumber == target)
        {
            stringSoFar = $"{stringSoFar} + {currentNumber}";

            FoundSolution($"{stringSoFar}");
        }
    }

    private static void FoundSolution(string solution)
    {
        Console.WriteLine(solution);
    }
}