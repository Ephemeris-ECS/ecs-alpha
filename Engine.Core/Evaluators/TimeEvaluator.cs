using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;

namespace Engine.Evaluators
{
	// ReSharper disable once InconsistentNaming
	public class TimeEvaluator<TECS, TConfiguration> : IEvaluator<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public int Threshold { get; set; }

		private DateTime _value;

		public void Activate()
		{
			_value = DateTime.Now;
		}

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			return DateTime.Now.Subtract(_value).TotalMilliseconds >= Threshold;
		}
	}
}
