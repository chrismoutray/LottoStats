using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LottoStats.Core.Interfaces
{
	public interface ICalculator
	{
		string Name { get; }

		bool Enabled { get; }

        void Populate(DrawHistory drawHistory);

        void Populate(Draw draw);

		void Calculate();

		bool Validate(Draw latest, Draw predicted);
	}
}
