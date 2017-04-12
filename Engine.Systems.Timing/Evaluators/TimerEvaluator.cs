using System;
using Engine.Configuration;
using Engine.Evaluators;
using Engine.Sequencing;

namespace Engine.Systems.Timing.Evaluators
{
	// ReSharper disable once InconsistentNaming
	public class TimerEvaluator<TECS, TConfiguration> : IEvaluator<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		private TimerSystem _timerSystem;

		public void Initialize(TECS ecs, TConfiguration configuration)
		{
			if (ecs.TryGetSystem(out _timerSystem) == false)
			{
				throw new SequenceException("TimerEvaluator failed to initialize: TimerSystem not registered.");
			}
		}

		public bool Evaluate(TECS ecs, TConfiguration configuration)
		{
			return _timerSystem.Current.Ticks == 0;
		}

		public void Dispose()
		{
			// do nothing

		}

	}
}
