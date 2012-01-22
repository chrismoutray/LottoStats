using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LottoStats.Core.Plugins
{
    /// <summary>
    /// Assert at least one of the predicted set is greater than 31.
    /// </summary>
	class OccuranceOfNumbersGT3 : LottoStats.Core.Interfaces.ICalculator
	{
		private SortedList<string, int> list = new SortedList<string, int>();

		private int total = 0;

		#region ICalculator Members
		
		public string Name
		{
			get { return this.GetType().Name; }
		}

		public bool Enabled
		{
            get { return true; } //always false
		}

        public void Populate(DrawHistory draws)
        {
            foreach (Draw draw in draws)
                Populate(draw);
        }

		public void Populate(Draw draw)
		{
			total++;

			for (int i = 1; i <= 6; i++)
			{
				foreach (List<int> sets in GetSets(draw, i))
				{
					string key = string.Join(",", sets.Select(x => x.ToString("00")).ToArray());

					key = i + "|" + key;

					if (!list.ContainsKey(key))
						list.Add(key, 0);
	
					list[key]++;
				}
			}
		}

		public void Calculate()
		{
			foreach (KeyValuePair<string, int> item in list.ToList())
			{
				if (item.Key.StartsWith("4|") && item.Value > 2)
					Console.WriteLine(item.Key + " - " + item.Value);
			}
		}

		public bool Validate(Draw latest, Draw predicted)
		{
			return true;
		}

		#endregion

		private IEnumerable<List<int>> GetSets(Draw draw, int s)
		{
			List<int> balls = draw.BallSorted;

			for (int a = 0; a < 6; a++)
			{
				for (int b = a+1; b < 6; b++)
				{
					for (int c = b+1; c < 6; c++)
					{
						for (int d = c+1; d < 6; d++)
						{
							for (int e = d+1; e < 6; e++)
							{
								for (int f = e+1; f < 6; f++)
								{
									if (s == 6)
										yield return new List<int>() { balls[a], balls[b], balls[c], balls[d], balls[e], balls[f] };
								}
								if (s == 5)
									yield return new List<int>() { balls[a], balls[b], balls[c], balls[d], balls[e] };
							}
							if (s == 4)
								yield return new List<int>() { balls[a], balls[b], balls[c], balls[d] };
						}
						if (s == 3)
							yield return new List<int>() { balls[a], balls[b], balls[c] };
					}
					if (s == 2)
						yield return new List<int>() { balls[a], balls[b] };
				}
				if (s == 1)
					yield return new List<int>() { balls[a] };
			}
		}
	}
}
