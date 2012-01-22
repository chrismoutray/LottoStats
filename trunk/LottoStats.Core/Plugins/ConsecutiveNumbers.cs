using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LottoStats.Core.Plugins
{
    /// <summary>
    /// Assert predicted set falls within top 2 highest percentage of no. of consecutive balls
    /// eg set 4,5,16,20,31,45 has 1 consecutive set 4 and 5
    /// set 4,10,11,21,22,45 has 2 consecutive sets 10&11 and 21&22
    /// so far on average drawn sets have either 0 or 1 consecutive sets
    /// </summary>
	class ConsecutiveNumbers : LottoStats.Core.Interfaces.ICalculator
	{
		private Dictionary<string, int> _ConsecutiveNumbers = new Dictionary<string, int>();

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
			counts[ConsecutiveNumberCount(draw)]++;
		}

		public void Calculate()
		{
			Console.Write("ConsecutiveNumbers (n/%): ");

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
		}

        int tm1 = 0;
        double tp1 = 0;
        int tm2 = 0;
        double tp2 = 0;

		public bool Validate(Draw latest, Draw predicted)
		{
			int cnt = ConsecutiveNumberCount(predicted);

			return (cnt == tm1 || cnt == tm2);
		}

		private int ConsecutiveNumberCount(Draw draw)
		{
			int count = 0;

            List<int> balls = draw.BallSorted;

			for (int i = 0; i < 5; i++)
			{
                int current = balls[i];
                int next = balls[i+1];
				if (current == next - 1)
					count++;
			}

			return count;
		}

		#endregion
	}
}
