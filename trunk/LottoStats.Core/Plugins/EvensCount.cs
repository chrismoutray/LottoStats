using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LottoStats.Core.Plugins
{
    class EvensCount : LottoStats.Core.Interfaces.ICalculator
    {
        int totalDraws = 0;
        
        int[] evenTotals = { 0, 0, 0, 0, 0, 0, 0 };
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

        public void Populate(DrawHistory drawHistory)
        {
            foreach (Draw draw in drawHistory)
                Populate(draw);
        }

        public void Populate(Draw draw)
        {
            totalDraws++;

            int evenCount = GetEvenCount(draw);

            evenTotals[evenCount]++;
        }

        private int GetEvenCount(Draw draw)
        {
            int evenCount = 0;

            foreach (int ball in draw.Ball)
            {
                if (IsEven(ball))
                    evenCount++;
            }

            return evenCount;
        }

        private bool IsEven(int num)
        {
            return (num % 2 == 0);
        }

        public void Calculate()
        {
            Console.Write(Name + " (n/%): ");

            for (int i = 0; i < evenTotals.Count(); i++)
            {
                percents[i] = ((double)evenTotals[i] / (double)totalDraws * 100);
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
                if ((i != tm1) && (i != tm2))
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
            int cnt = GetEvenCount(predicted);

            return (cnt == tm1 || cnt == tm2 || cnt == tm3);
        }

        #endregion
    }
}
