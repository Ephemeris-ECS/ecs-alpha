using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Evaluators;
using Zenject;

// ReSharper disable InconsistentNaming

namespace Engine.Sequencing
{
	public class ConditionalAction<TECS, TConfiguration> : ECSAction<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public IEvaluator<TECS, TConfiguration> Evaluator { get; set; }

		[Inject]
		public ConditionalAction()
		{
		}

		public ConditionalAction(ECSAction<TECS, TConfiguration> action, IEvaluator<TECS, TConfiguration> evaluator)
			: base (action.Action, action.Name)
		{
			Evaluator = evaluator;
		}

		#region Overrides of ECSAction<TECS,TConfiguration>

		public override void Execute(TECS ecs, TConfiguration configuration)
		{
			if (Evaluator?.Evaluate(ecs, configuration) ?? true)
			{
				base.Execute(ecs, configuration);
			}
		}

		#endregion
	}
}
