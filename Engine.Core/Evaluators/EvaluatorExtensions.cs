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
			return new LogicalOperationEvaluator<TECS, TConfiguration>(Operation.And, evaluator, and);
		}

		public static IEvaluator<TECS, TConfiguration> Or<TECS, TConfiguration>(this IEvaluator<TECS, TConfiguration> evaluator, IEvaluator<TECS, TConfiguration> or)
			where TECS : class, IECS
			where TConfiguration : ECSConfiguration
		{
			return new LogicalOperationEvaluator<TECS, TConfiguration>(Operation.Or, evaluator, or);
		}
		public static IEvaluator<TECS, TConfiguration> Not<TECS, TConfiguration>(IEvaluator<TECS, TConfiguration> evaluator)

			where TECS : class, IECS
			where TConfiguration : ECSConfiguration
		{
			return new LogicalOperationEvaluator<TECS, TConfiguration>(Operation.Not, evaluator);
		}

	}
}
