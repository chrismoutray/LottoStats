using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LottoStats.Core.Plugins
{
	/// <summary>
	/// Assert at least one predicted ball is greater than 31
	/// </summary>
	class AtLeast1GT31 : LottoStats.Core.Interfaces.ICalculator
	{
        int total = 0;
        int[] counts = { 0, 0, 0, 0, 0, 0, 0 };
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
            total++;
            counts[NumberGT31(draw)]++;
		}

        public void Calculate()
        {
            Console.Write("NumberGT31 (n/%): ");

            for (int i = 0; i < counts.Count(); i++)
            {
                percents[i] = ((double)counts[i] / (double)total * 100);

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

            for (int i = 0; i < percents.Count(); i++)
            {
                if (i != tm1 && i != tm2)
                    if (percents[i] > tp3)
                    {
                        tp3 = percents[i];
                        tm3 = i;
                    }
            }
        }

        int tm1 = 0;
        double tp1 = 0;
        int tm2 = 0;
        double tp2 = 0;
        int tm3 = 0;
        double tp3 = 0;

		public bool Validate(Draw latest, Draw predicted)
		{
            int cnt = NumberGT31(predicted);

            return (cnt == tm1 || cnt == tm2 || cnt == tm3);
		}

		#endregion

        private int NumberGT31(Draw draw)
        {
            int cnt = 0;
            foreach (int ball in draw.Ball)
                if (ball > 31)
                    cnt++;
            return cnt;
        }
	}
}
