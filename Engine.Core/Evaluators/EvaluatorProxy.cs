using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;

namespace Engine.Evaluators
{
	// ReSharper disable once InconsistentNaming
	public class EvaluatorProxy<TECS, TConfiguration> : IEvaluator<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		protected Func<TECS, TConfiguration, bool> Evaluator;

		public EvaluatorProxy()
		{
		}

		public EvaluatorProxy(Func<TECS, TConfiguration, bool> evaluator)
		{
			Evaluator = evaluator;
		}
		public void Initialize(TECS ecs, TConfiguration configuration)
		{
			
		}

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			return Evaluator(ecs, configuration);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

	}
}
