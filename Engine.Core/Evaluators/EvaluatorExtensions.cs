using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;

namespace Engine.Evaluators
{
	public static class EvaluatorExtensions
	{
		public static IEvaluator<TECS, TConfiguration> And<TECS, TConfiguration>(this IEvaluator<TECS, TConfiguration> evaluator, IEvaluator<TECS, TConfiguration> and)
			where TECS : class, IECS
			where TConfiguration : ECSConfiguration
		{
			return new CompoundEvaluator<TECS, TConfiguration>(Operand.And, new [] { evaluator, and });
		}

		public static IEvaluator<TECS, TConfiguration> Or<TECS, TConfiguration>(this IEvaluator<TECS, TConfiguration> evaluator, IEvaluator<TECS, TConfiguration> or)
			where TECS : class, IECS
			where TConfiguration : ECSConfiguration
		{
			return new CompoundEvaluator<TECS, TConfiguration>(Operand.Or, new[] { evaluator, or });
		}
	}
}
