using System;
using System.Threading;
using Engine.Configuration;
using Engine.Sequencing;

// ReSharper disable InconsistentNaming

namespace Engine.Lifecycle
{
	public class ECSRunner<TECS, TConfiguration> : IDisposable
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public event Action Tick;

		public event Action<Exception> Exception;

		private readonly int _tickInterval;

		private readonly Sequencer<TECS, TConfiguration> _sequencer;

		private readonly TECS _ecs;

		private readonly ManualResetEvent _abortSignal = new ManualResetEvent(false);
		private readonly AutoResetEvent _continueSignal = new AutoResetEvent(false);

		private Thread _ecsLoopThread;

		public ECSRunner(int tickInterval, Sequencer<TECS, TConfiguration> sequencer, TECS ecs)
		{
			_tickInterval = tickInterval;
			_sequencer = sequencer;
			_ecs = ecs;
		}

		public void Start()
		{
			if (_ecsLoopThread == null)
			{
				_ecsLoopThread = new Thread(ECSLoop)
				{
					IsBackground = true,
				};
				_ecsLoopThread.Start();
			}
		}

		public void Stop()
		{
			_abortSignal.Set();
			_ecsLoopThread.Join(10000);
		}

		private void ECSLoop(object state)
		{
			DateTime lastLoop;
			while (true)
			{
				lastLoop = DateTime.Now;
				try
				{
					if (_sequencer?.IsComplete ?? false)
					{
						Stop();
					}
					else
					{
						_ecs.Tick();
						_sequencer?.Tick(_ecs);
						OnTick();
						WaitHandle.WaitAny(new WaitHandle[] {_continueSignal, _abortSignal});
					}
				}
				catch (Exception ex)
				{
//					throw new LifecycleException("Error in ECS runner loop", ex);
					_abortSignal.Set();
					OnException(ex);
				}
				var sleep = Math.Max(0, _tickInterval - (int)DateTime.Now.Subtract(lastLoop).TotalMilliseconds);
				if (_abortSignal.WaitOne(sleep))
				{
					break;
				}
			}
		}

		protected virtual void OnTick()
		{
			Tick?.Invoke();
			_continueSignal.Set();
		}

		public void Dispose()
		{
			Stop();
			((IDisposable) _abortSignal)?.Dispose();
			((IDisposable) _continueSignal)?.Dispose();
		}

		protected virtual void OnException(Exception obj)
		{
			Exception?.Invoke(obj);
		}
	}
}
