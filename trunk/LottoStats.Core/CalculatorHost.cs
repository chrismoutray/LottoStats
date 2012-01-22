using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LottoStats.Core.Interfaces;
using LottoStats.Core.Attributes;

namespace LottoStats.Core
{
	public class CalculatorHost
	{
		private ICalculator _calculator;

		public CalculatorHost(ICalculator calculator)
		{
			_calculator = calculator;
		}

		public string Name
		{
			get { return _calculator.Name; }
		}

		public bool Enabled
		{
			get { return _calculator.Enabled; }
		}

        public void Populate(DrawHistory drawHistory)
        {
            _calculator.Populate(drawHistory);
        }

		public void Calculate()
		{
			_calculator.Calculate();
		}

		public bool Validate(Draw latest, Draw predicted)
		{
			return _calculator.Validate(latest, predicted);
		}

		public override string ToString()
		{
			object[] arr = _calculator.GetType().GetCustomAttributes(typeof(CalculationPlugInAttribute), true);

			return "\n" + ((CalculationPlugInAttribute)arr[0]).Description + "\n\n" + _calculator.ToString();
		}
	}
}
