using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LottoStats.Core.Attributes;

namespace LottoStats.Core
{
    /// <summary>
    /// Assert predicted set falls within top 2 highest percentage of no. of balls seen 2 weeks running
    /// ie on average a draw always contains 2 numbers from the prev week then we need to make sure our prediced number contains at least 2 numbers from prev draw
    /// </summary>
	class TwoWeeksRunning : LottoStats.Core.Interfaces.ICalculator
	{
		int totalDraws = 0;
		Draw PrevWeeksDraw;

		// number of balls seen in prev draw; count for each match 
		// eg 0 balls seen last draw
		// 1 ball seen last draw
		// 2 balls seen last draw etc
		int[] counts = { 0, 0, 0, 0, 0, 0, 0 };
		// percent of number seen against total number of draws
		double[] percents = { 0, 0, 0, 0, 0, 0, 0 };

		#region ICalculator Members

		public string Name
		{
			get { return this.GetType().Name; }
		}

		public bool Enabled
		{
            get { return true; }
		}

        public void Populate(DrawHistory draws)
        {
            foreach (Draw draw in draws)
                Populate(draw);
        }

        public void Populate(Draw draw)
		{
			totalDraws++;

			if (PrevWeeksDraw != null)
				counts[draw.Matches(PrevWeeksDraw)]++;

			PrevWeeksDraw = draw;
		}

		public void Calculate()
		{
			Console.Write("TwoWeeksRunning (n/%): ");

			for (int i = 0; i < counts.Count(); i++)
			{
				percents[i] = ((double)counts[i] / (double)totalDraws * 100);
				Console.Write(string.Format("{0}:{1:0.0}% ", i, percents[i]));
			}

			Console.WriteLine();

            for (int i = 0; i < percents.Count(); i++)
            {
                if (percents[i] > tp1)
                {
                    tp1 = percents[i];
                    tm1 = i;
                }
            }

            for (int i = 0; i < percents.Count(); i++)
            {
                if (i != tm1)
                    if (percents[i] > tp2)
                    {
                        tp2 = percents[i];
                        tm2 = i;
                    }
            }
		}

        int tm1 = 0;
        double tp1 = 0;
        int tm2 = 0;
        double tp2 = 0;

		public bool Validate(Draw latest, Draw predicted)
		{
			int m = predicted.Matches(latest);
            
			return (m == tm1 || m == tm2);
		}

		#endregion
	}
}
