using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Evaluators
{
	public class EvaluatorProxy<TECS> : IECSEvaluator<TECS>
		where TECS : class, IECS
	{
		private Func<TECS, bool> _evaluator;

		public EvaluatorProxy(Func<TECS, bool> evaluator)
		{
			_evaluator = evaluator;
		}

		public bool Evaluate(TECS ecs)
		{
			return _evaluator(ecs);
		}
	}
}
