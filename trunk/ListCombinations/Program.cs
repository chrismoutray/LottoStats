using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LottoStats.Core;
using System.Threading.Tasks;

namespace ListCombinations
{
    class Program
    {
        static void Main(string[] args)
        {
            DrawHistory draws = DrawHistory.GetDrawHistory();

            List<CalculatorHost> calculators = CalculatorHostProvider.Calculators;

            calculators.ForEach(c => c.Populate(draws));
            calculators.ForEach(c => c.Calculate());

            Draw lastDraw = draws.Last();

            AllCombinations(calculators, lastDraw);

            AllDraws(draws, calculators, lastDraw);

            Console.ReadLine();
        }

        private static void AllDraws(DrawHistory draws, List<CalculatorHost> calculators, Draw lastDraw)
        {
            int totalDraws = draws.Count;

            Console.WriteLine("Draws");
            Console.WriteLine("All " + totalDraws);

            int validDraws = 0;
            int invalidDraws = 0;

            var list = draws.GetRange(0, draws.Count - 1);

            foreach (Draw draw in list)
            {
                bool isValid = true;

                foreach (CalculatorHost calculator in calculators)
                {
                    if (!calculator.Validate(lastDraw, draw))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    validDraws++;
                else
                    invalidDraws++;
            }

            Console.WriteLine("Valid " + validDraws + " - " + (((double)validDraws / (double)totalDraws) * 100));
            Console.WriteLine("Invaild " + invalidDraws + " - " + (((double)invalidDraws / (double)totalDraws) * 100));
        }

        private static void AllCombinations(List<CalculatorHost> calculators, Draw lastDraw)
        {
            List<List<int>> allCombinations = GetAllCombinations();

            int totalCombinations = allCombinations.Count;

            Console.WriteLine("Combinations");
            Console.WriteLine("All " + totalCombinations);

            int validCombinations = 0;
            int invalidCombinations = 0;

            foreach (List<int> combination in allCombinations)
            {
                Draw draw = new Draw(combination);

                bool isValid = true;

                foreach (CalculatorHost calculator in calculators)
                {
                    if (!calculator.Validate(lastDraw, draw))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                    validCombinations++;
                else
                    invalidCombinations++;
            }

            Console.WriteLine("Valid " + validCombinations + " - " + (((double)validCombinations / (double)totalCombinations) * 100));
            Console.WriteLine("Invaild " + invalidCombinations + " - " + (((double)invalidCombinations / (double)totalCombinations) * 100));
        }

        private static List<List<int>> GetAllCombinations()
        {
            List<List<int>> balls = new List<List<int>>();

            for (int a = 1; a <= 44; a++)
                for (int b = a + 1; b <= 45; b++)
                    for (int c = b + 1; c <= 46; c++)
                        for (int d = c + 1; d <= 47; d++)
                            for (int e = d + 1; e <= 48; e++)
                                for (int f = e + 1; f <= 49; f++)
                                    balls.Add(new List<int>() { a, b, c, d, e, f });

            return balls;
        }
    }
}
