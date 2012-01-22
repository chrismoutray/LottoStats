using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LottoStats.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CalculationPlugInAttribute : Attribute
	{
		public string Description
		{
			get;
			set;
		}

		public CalculationPlugInAttribute(string description)
		{
			Description = description;
		}
	}
}
