using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Evaluators
{
	public class TickEvaluator<TECS> : IECSEvaluator<TECS>
		where TECS : class, IECS
	{
		public int Threshold { get; set; }

		private int _value;

		public void Activate()
		{
			_value = 0;
		}

		public bool Evaluate(TECS ecs)
		{
			return _value++ >= Threshold;
		}
	}
}
