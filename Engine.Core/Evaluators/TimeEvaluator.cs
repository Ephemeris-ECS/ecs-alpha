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

		private DateTime _value = DateTime.MinValue;
		public void Initialize(TECS ecs, TConfiguration configuration)
		{
			// do nothing
		}

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			if (_value == DateTime.MinValue)
			{
				_value = DateTime.Now;
			}

			var success = DateTime.Now.Subtract(_value).TotalMilliseconds >= Threshold;
			if (success)
			{
				_value = DateTime.MinValue;
			}
			return success;
		}

		public void Dispose()
		{
			// do nothing

		}

	}
}
