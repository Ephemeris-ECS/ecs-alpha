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

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			var success = _value++ >= Threshold;
			if (success)
			{
				_value = 0;
			}
			return success;
		}

		#region Implementation of IEvaluator<in TECS,in TConfiguration>

		public void Initialize(TECS ecs, TConfiguration configuration)
		{
			// do nothing
		}

		public void Dispose()
		{
			// do nothing
		}

		#endregion
	}
}
