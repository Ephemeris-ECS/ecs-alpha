using System;
using System.Linq;
using System.Security.Permissions;
using Engine.Configuration;

namespace Engine.Evaluators
{
	// ReSharper disable once InconsistentNaming
	public class LogicalOperationEvaluator<TECS, TConfiguration> : IEvaluator<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		private readonly Operation _operation;

		private readonly IEvaluator<TECS, TConfiguration>[] _evaluators;

		public LogicalOperationEvaluator(Operation operation, params IEvaluator<TECS, TConfiguration>[] evaluators)
		{
			_operation = operation;
			_evaluators = evaluators;
		}

		public void Initialize(TECS ecs, TConfiguration configuration)
		{
			foreach (var evaluator in _evaluators)
			{
				evaluator.Initialize(ecs, configuration);
			}
		}

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			switch (_operation)
			{
				case Operation.And:
					return _evaluators.All(e => e.Evaluate(ecs, configuration));
				case Operation.Or:
					return _evaluators.Any(e => e.Evaluate(ecs, configuration));
				case Operation.Not:
					return (_evaluators.SingleOrDefault()?.Evaluate(ecs, configuration) ?? false) == false;
			}
			return false;
		}

		public void Dispose()
		{
			foreach (var evaluator in _evaluators)
			{
				evaluator.Dispose();
			}
		}

	}
}
