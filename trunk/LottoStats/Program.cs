using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LottoStats.Core;
using System.Diagnostics;
using System.Threading.Tasks;
namespace LottoStats
{
	class Program
	{
		static void Main(string[] args)
		{
			DrawHistory draws = DrawHistory.GetDrawHistory();

            ShowMissingDrawHistory(draws);

			List<CalculatorHost> calculators = CalculatorHostProvider.Calculators;

            calculators.ForEach(c => c.Populate(draws));
            
            calculators.ForEach(c => c.Calculate());

            Draw lastDraw = draws[draws.Count - 1];
            Draw prevToLastDraw = draws[draws.Count - 2];

			Console.WriteLine();
			Console.WriteLine("Last draw : " + lastDraw);

            ValidateLastDrawAgainstPrevious(calculators, lastDraw, prevToLastDraw);

            int rechooseIfGtEqMatchThreshold = 5;

			Console.WriteLine();
            Console.WriteLine("Match threshold is " + rechooseIfGtEqMatchThreshold.ToString() + "; Sets of " + rechooseIfGtEqMatchThreshold.ToString() + " or more that have been drawn in the past are excluded from our predicted list.");
            Console.WriteLine();

            int drawn = 0;
			while (true)
			{
                OutputNextSet(draws, calculators, rechooseIfGtEqMatchThreshold);

				if (drawn == 10)
				{
					Console.ReadLine();
					drawn = 0;
				}
				drawn++;
			}
		}

        private static void ValidateLastDrawAgainstPrevious(List<CalculatorHost> calculators, Draw lastDraw, Draw prevToLastDraw)
        {
            StringBuilder sb = new StringBuilder();

            foreach (CalculatorHost calculator in calculators)
            {
                if (!calculator.Validate(prevToLastDraw, lastDraw))
                {
                    if (sb.Length > 0)
                        sb.Append("; ");

                    sb.Append(calculator.Name);
                }
            }

            if (sb.Length > 0)
            {
                Console.WriteLine("Prev to last: " + prevToLastDraw.ToString() + "");
                Console.WriteLine("Last draw failed: ");
                sb.Insert(0, "\t");
                Console.WriteLine(sb.ToString());
            }
        }

        private static void ShowMissingDrawHistory(DrawHistory dh)
        {
            List<DateTime> missing = dh.GetMissingHistory();

            if (missing != null && missing.Count > 0)
            {
                Console.WriteLine("Missing Draw Draw History:");
                foreach (DateTime date in missing)
                    Console.WriteLine("\t" + date.ToLongDateString() + " (" + date.DayOfWeek.ToString() + ")");
                Console.WriteLine();
            }
        }

        private static void OutputNextSet(DrawHistory dh, List<CalculatorHost> calculators, int matchesBeforeFail)
        {
            Draw d = null;
            bool valid = true;

            do
            {
                d = dh.Next(matchesBeforeFail);
                valid = true;
                 
                //d = new Draw(new List<int> { 15, 21, 26, 30, 47, 48 });

                foreach (CalculatorHost calculator in calculators)
                {
                    valid = calculator.Validate(dh.Last(), d);

                    if (!valid)
                        break;
                }
            }
            while (!valid);

            Console.WriteLine(d.ToString());
        }
	}
}
