using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Evaluators
{
	public class TimeEvaluator<TECS> : IECSEvaluator<TECS>
		where TECS : class, IECS
	{
		public int Threshold { get; set; }

		private DateTime _value;

		public void Activate()
		{
			_value = DateTime.Now;
		}

		public bool Evaluate(TECS ecs)
		{
			return DateTime.Now.Subtract(_value).TotalMilliseconds >= Threshold;
		}
	}
}
