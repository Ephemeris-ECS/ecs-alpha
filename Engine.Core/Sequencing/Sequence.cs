using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModestTree;

namespace Engine.Sequencing
{
	/// <summary>
	/// The basic sequence implementation is a linear, forward only state machine
	/// each frame is executed until the condition is satisfied then it is progresses
	/// in the long term this will become a graph with multiple routes and cyclic states and transitions
	/// </summary>
	// ReSharper disable once InconsistentNaming
	public class Sequence<TECS>
		where TECS : class, IECS
	{
		public SequenceFrame<TECS>[] Frames { get; set; }

		private SequenceFrame<TECS> _currentFrame;

		private readonly TECS _ecs;

		private int _frameIndex = 0;

		public bool IsComplete { get; private set; }

		public event Action Complete;

		public Sequence(TECS ecs)
		{
			Assert.IsNotNull(ecs);
			_ecs = ecs;
		}

		public void Tick()
		{
			if (_currentFrame == null && _frameIndex < Frames.Length)
			{
				_currentFrame = Frames[_frameIndex];
				_currentFrame.Enter(_ecs);
			}
			else if (_currentFrame?.Evaluator.Evalulate(_ecs) ?? false)
			{
				_currentFrame.Exit(_ecs);
				_currentFrame = null;

				if (++_frameIndex >= Frames.Length)
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
