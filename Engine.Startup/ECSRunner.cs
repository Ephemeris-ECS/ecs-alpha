using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Engine.Sequencing;

// ReSharper disable InconsistentNaming

namespace Engine.Startup
{
	public class ECSRunner<TECS>
		where TECS : class, IECS
	{
		private int _tickInterval = 100;

		private readonly Sequencer<TECS> _sequencer;

		private readonly TECS _ecs;

		private readonly Timer _tickTimer;

		public ECSRunner(int tickInterval, Sequencer<TECS> sequencer, TECS ecs)
		{
			_tickInterval = tickInterval;
			_sequencer = sequencer;
			_ecs = ecs;
			_tickTimer = new Timer(ECSLoop, null, Timeout.Infinite, Timeout.Infinite);
		}

		public void Start()
		{
			_tickTimer.Change(0, _tickInterval);
		}

		public void Stop()
		{
			_tickTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void ECSLoop(object state)
		{
			if (_sequencer.IsComplete)
			{
				Stop();
			}
			else
			{
				_sequencer.Tick();
				_ecs.Tick();
			}
		}
	}
}
