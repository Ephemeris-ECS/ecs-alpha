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
		private SequenceFrame<TECS> _currentFrame;

		private readonly Scenario<TECS, TConfiguration> _scenario;

		private int _frameIndex = 0;

		public bool IsComplete { get; private set; }

		public event Action Complete;

		public Sequencer(Scenario<TECS, TConfiguration> scenario)
		{
			Assert.IsNotNull(scenario);
			_scenario = scenario;

			Assert.IsNotNull(scenario.Sequence);
		}

		public void Tick(TECS ecs)
		{
			if (_currentFrame == null && _frameIndex < _scenario.Sequence.Length)
			{
				_currentFrame = _scenario.Sequence[_frameIndex];
				_currentFrame.Enter(ecs);
			}
			else if (_currentFrame?.Evaluator.Evaluate(ecs) ?? false)
			{
				_currentFrame.Exit(ecs);
				_currentFrame = null;

				if (++_frameIndex >= _scenario.Sequence.Length)
				{
					OnComplete();
				}
			}
		}

		protected virtual void OnComplete()
		{
			Complete?.Invoke();
		}
	}
}
