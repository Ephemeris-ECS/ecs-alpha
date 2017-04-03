using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using ModestTree;

namespace Engine.Sequencing
{
	/// <summary>
	/// The basic sequence implementation is a linear, forward only state machine
	/// each frame is executed until the condition is satisfied then it is progresses
	/// in the long term this will become a graph with multiple routes and cyclic states and transitions
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class Sequencer<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		private SequenceFrame<TECS, TConfiguration> _currentFrame;

		private readonly Scenario<TECS, TConfiguration> _scenario;

		private int _frameIndex = 0;

		public bool IsComplete { get; private set; }

		public Sequencer(Scenario<TECS, TConfiguration> scenario)
		{
			Assert.IsNotNull(scenario);
			_scenario = scenario;

			Assert.IsNotNull(scenario.Sequence);
		}

		public void Tick(TECS ecs, TConfiguration configuration)
		{
			if (_currentFrame == null && _frameIndex < _scenario.Sequence.Count)
			{
				_currentFrame = _scenario.Sequence[_frameIndex];
				_currentFrame.Enter(ecs, configuration);
			}
			else if (_currentFrame?.Evaluator.Evaluate(ecs, configuration) ?? false)
			{
				_currentFrame.Exit(ecs, configuration);
				_currentFrame = null;

				if (++_frameIndex >= _scenario.Sequence.Count)
				{
					OnComplete();
				}
			}
			else
			{
				_currentFrame?.Tick(ecs, configuration);
			}
		}

		protected virtual void OnComplete()
		{
			IsComplete = true;

		}
	}
}
