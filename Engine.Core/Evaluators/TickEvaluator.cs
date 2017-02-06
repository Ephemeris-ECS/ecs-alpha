using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;

namespace Engine.Evaluators
{
	// ReSharper disable once InconsistentNaming
	public class TickEvaluator<TECS, TConfiguration> : IEvaluator<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public int Threshold { get; set; }

		private int _value;

		public void Activate()
		{
			_value = 0;
		}

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			return _value++ >= Threshold;
		}
	}
}
