using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using LottoStats.Core.Interfaces;
using LottoStats.Core.Attributes;

namespace LottoStats.Core
{
	public class CalculatorHostProvider
	{
		private static List<CalculatorHost> _calculators;

		public static List<CalculatorHost> Calculators
		{
			get
			{
				if (null == _calculators)
					Reload();

				return _calculators;
			}
		}

		public static void Reload()
		{

			if (null == _calculators)
				_calculators = new List<CalculatorHost>();
			else
				_calculators.Clear();

			//_calculators.Add(new TwoWeeksRunning()); // load the default
			List<Assembly> plugInAssemblies = LoadPlugInAssemblies();
			List<ICalculator> plugIns = GetPlugIns(plugInAssemblies);

			foreach (ICalculator calc in plugIns)
			{
				if (calc.Enabled)
					_calculators.Add(new CalculatorHost(calc));
			}
		}

		private static List<Assembly> LoadPlugInAssemblies()
		{
			DirectoryInfo dInfo = new DirectoryInfo(new FileInfo(typeof(ICalculator).Assembly.Location).DirectoryName);
			FileInfo[] files = dInfo.GetFiles("*.dll");

			List<Assembly> plugInAssemblyList = new List<Assembly>();

			if (null != files)
			{
				foreach (FileInfo file in files)
					plugInAssemblyList.Add(Assembly.LoadFile(file.FullName));
			}

			return plugInAssemblyList;
		}

		static List<ICalculator> GetPlugIns(List<Assembly> assemblies)
		{
			List<Type> availableTypes = new List<Type>();

			foreach (Assembly currentAssembly in assemblies)
				availableTypes.AddRange(currentAssembly.GetTypes());

			// get a list of objects that implement the ICalculator interface AND have the CalculationPlugInAttribute
			List<Type> calculatorList = availableTypes.FindAll(delegate(Type t)
				{
					return (new List<Type>(t.GetInterfaces())).Contains(typeof(ICalculator));
				});

			// conver the list of Objects to an instantiated list of ICalculators
			return calculatorList.ConvertAll<ICalculator>(delegate(Type t) { return Activator.CreateInstance(t) as ICalculator; });
		}
	}
}
