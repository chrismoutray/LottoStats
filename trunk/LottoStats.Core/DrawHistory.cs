using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography;

namespace LottoStats.Core
{
	public class DrawHistory : List<Draw>
	{
		private Dictionary<DateTime, Draw> dictionary = new Dictionary<DateTime, Draw>();
		//private Random rnd = new Random(DateTime.Now.Millisecond);

        RNGCryptoServiceProvider rnd2 = new RNGCryptoServiceProvider();

		public static DrawHistory GetDrawHistory()
		{
			return new DrawHistory();
		}

		public DrawHistory()
		{			
			string url = "http://www.national-lottery.co.uk/player/files/Lotto.csv";

			DateTime date = DateTime.Now.Date;

			string filename = string.Format("DrawHistory_{0:0000}{1:00}{2:00}.csv", date.Year, date.Month, date.Day);

			if (!File.Exists(filename))
			{
				WebClient webClient = new WebClient();

				Console.WriteLine("Downloading: " + url);
				Console.WriteLine();

				try
				{
					webClient.DownloadFile(url, filename);
					Console.WriteLine("Successfully download...");
				}
				catch (Exception ex)
				{
					Console.WriteLine("Download failed: " + ex.Message);
				}
				
				Console.WriteLine();
			}

			LoadHistory("DrawHistory.csv");

			LoadHistory(filename);

			SaveHistory("DrawHistory.csv");
		}

		public void SaveHistory(string filename)
		{
			File.Copy(filename, string.Format("DrawHistory_{0:0000}{1:00}{2:00}.csv.bak", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day), true); 
			
			using (StreamWriter sw = new StreamWriter(filename))
			{
				foreach (Draw draw in this)
				{
					sw.WriteLine(draw.ToLongString());
				}
			}
		}

		public void LoadHistory(string filename)
		{
			if (!File.Exists(filename))
				return;

			using (StreamReader sr = new StreamReader(filename))
			{
				while (!sr.EndOfStream)
				{
					string line = sr.ReadLine();

					string[] lineContent = line.Split(',');

					int i;
					if (lineContent.Length == 10 && int.TryParse(lineContent[1], out i))
					{
						Draw d = new Draw();

						d.Date = DateTime.Parse(lineContent[0]);

						//if (!this.Contains<Draw>(d, new DrawEqualityComparer()))
						
						if (!dictionary.ContainsKey(d.Date))
						{
							d.Ball.Add(int.Parse(lineContent[1]));
							d.Ball.Add(int.Parse(lineContent[2]));
							d.Ball.Add(int.Parse(lineContent[3]));
							d.Ball.Add(int.Parse(lineContent[4]));
							d.Ball.Add(int.Parse(lineContent[5]));
							d.Ball.Add(int.Parse(lineContent[6]));

							int.TryParse(lineContent[7], out i);
							d.BonusBall = i;

							int.TryParse(lineContent[8], out i);
							d.BallSet = i;

							d.Machine = lineContent[9].Trim();

							this.Add(d);
							dictionary.Add(d.Date, d);
						}
					}
				}
			}

			// sort by date
			this.Sort(new DrawComparer());

			// set the draw number
			int cnt = this.Count;
			foreach (Draw d in this)
			{
				d.Number = cnt;
				cnt--;
			}
		}

		public Draw Next(int matchThreshold)
		{
			Draw d;

			bool failed = false;

			do
			{
				failed = false;
				d = NewDraw();

				foreach (Draw draw in this)
				{
					int m = d.Matches(draw);

                    if (m < matchThreshold)
                        continue;

                    failed = true;
                    break;
				}
			} while (failed == true);

			return d;
		}

		public Draw NewDraw()
		{
			Draw d = new Draw();

			d.Ball = new List<int>();

			do 
			{
                int i = NextRandomNumber();

				if (!d.Ball.Contains(i))
					d.Ball.Add(i);

			} while (d.Ball.Count < 6);

			return d;
		}

        private int NextRandomNumber()
        {
            //return rnd.Next(1, 49);

            byte[] r = new byte[4];
            rnd2.GetBytes(r);
            int result = Math.Abs(BitConverter.ToInt32(r, 0));
            return result % 49 + 1;
        }

		public List<DateTime> GetMissingHistory()
		{
			List<DateTime> missing = new List<DateTime>();

			DateTime firstEverDraw = new DateTime(1994, 11, 19);

			DateTime date = firstEverDraw;

			while (date < DateTime.Now.Date)
			{
				int adjust = 0;

				if (date == new DateTime(2004, 12, 25))
					adjust = -1;

				if (date == new DateTime(2002, 12, 25))
					adjust = -1;

				if (date == new DateTime(1999, 12, 25))
					adjust = -1;

				if (date == new DateTime(1997, 09, 06))
					adjust = 1;

				if (!dictionary.ContainsKey(date.AddDays(adjust)))
					if (date < new DateTime(1997, 09, 06) && IsWednesday(date))
					{
						// do nothing no wed draws expected...
					}
					else
						missing.Add(date.AddDays(adjust));

				date = NextDrawDate(date);
			}

			if (missing.Count == 0)
				return null;

			return missing;
		}

		private DateTime NextDrawDate()
		{
			return NextDrawDate(DateTime.Now);
		}

		private DateTime NextDrawDate(DateTime date)
		{
			date = date.Date.AddDays(1);

			while (!IsWednesday(date) && !IsSaturday(date))
			{
				date = date.AddDays(1);
			}

			return date;
		}

		private bool IsWednesday(DateTime date)
		{
			return (date.DayOfWeek == DayOfWeek.Wednesday);
		}

		private bool IsSaturday(DateTime date)
		{
			return (date.DayOfWeek == DayOfWeek.Saturday);
		}

	}

	public class DrawComparer : IComparer<Draw>
    {
        #region IComparer Members

        public int Compare(Draw x, Draw y)
        {
			return x.Date.CompareTo(y.Date);
        }

        #endregion
    }

	public class DrawEqualityComparer : IEqualityComparer<Draw>
	{
		#region IEqualityComparer<Draw> Members

		public bool Equals(Draw x, Draw y)
		{
			return x.Date.Equals(y.Date);
		}

		public int GetHashCode(Draw obj)
		{
			return obj.Date.GetHashCode();
		}

		#endregion
	}
}
