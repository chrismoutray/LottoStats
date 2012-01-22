using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LottoStats.Core
{
	public class Draw
	{
		// DrawDate	Ball 1	Ball 2	Ball 3	Ball 4	Ball 5	Ball 6	Bonus Ball	Ball Set	Machine

		#region Declarations

		#endregion

		#region Properties

		public int Number
		{
			get;
			set;
		}

		public DateTime Date
		{
			get;
			set;
		}

		public List<int> Ball
		{
			get;
			set;
		}

		public int BonusBall
		{
			get;
			set;
		}

		public int BallSet
		{
			get;
			set;
		}

		public string Machine
		{
			get;
			set;
		}

		#endregion

		#region Constructors

		public Draw()
		{
			Ball = new List<int>(6);
		}

        public Draw(List<int> balls)
        {
            Ball = balls;
        }

		public List<int> BallSorted
		{
			get
			{
				List<int> list = new List<int>();
				
				foreach (int i in Ball)
					list.Add(i);

				list.Sort();

				return list;
			}
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0:00}, {1:00}, {2:00}, {3:00}, {4:00}, {5:00}", Ball[0], Ball[1], Ball[2], Ball[3], Ball[4], Ball[5]);
		}

		public string ToStringSorted()
		{
			List<int> balls = BallSorted;

			return string.Format("{0:00}, {1:00}, {2:00}, {3:00}, {4:00}, {5:00}", balls[0], balls[1], balls[2], balls[3], balls[4], balls[5]);
		}

		public string ToLongString()
		{
			return string.Format("{0:yyyy-MMM-dd}, {1:00}, {2:00}, {3:00}, {4:00}, {5:00}, {6:00}, {7:00}, {8}, {9}", Date, Ball[0], Ball[1], Ball[2], Ball[3], Ball[4], Ball[5], BonusBall, BallSet, Machine);
		}

		public bool IsEqual(Draw draw)
		{
			bool flag = (draw.Ball.Count == this.Ball.Count);
			
			foreach (int i in this.Ball)
			{
				flag = flag && draw.Ball.Contains(i);
			}

			return flag;
		}

		public int Matches(Draw draw)
		{
			int m = 0;
			foreach (int i in this.Ball)
			{
				if (draw.Ball.Contains(i))
					m++;
			}

			return m;
		}
	}
}
