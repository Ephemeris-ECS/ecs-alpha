using System;
using System.Linq;
using Engine.Configuration;

namespace Engine.Evaluators
{
	// ReSharper disable once InconsistentNaming
	public class CompoundEvaluator<TECS, TConfiguration> : IEvaluator<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		private readonly Operand _operand;

		private readonly IEvaluator<TECS, TConfiguration>[] _evaluators;

		public CompoundEvaluator(Operand operand, IEvaluator<TECS, TConfiguration>[] evaluators)
		{
			_operand = operand;
			_evaluators = evaluators;
		}

		public void Activate()
		{
		}

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			switch (_operand)
			{
				case Operand.And:
					return _evaluators.All(e => e.Evaluate(ecs, configuration));
				case Operand.Or:
					return _evaluators.Any(e => e.Evaluate(ecs, configuration));
			}
			return false;
		}
	}
}
