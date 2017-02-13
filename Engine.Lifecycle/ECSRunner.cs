using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Engine.Commands;
using Engine.Configuration;
using Engine.Sequencing;

// ReSharper disable InconsistentNaming

namespace Engine.Lifecycle
{
	public delegate void ECSTick(Tick tick);

	// TODO: there should be a more configuration based approach to what happens in the loop
	// we may not want to process commands, serialize, sequence etc and these should be enabled in an extensible manner

	public class ECSRunner<TECS, TConfiguration> : IDisposable
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public event ECSTick Tick;

		public event Action<Exception> Exception;

		private readonly int _tickInterval;

		private readonly Sequencer<TECS, TConfiguration> _sequencer;

		private readonly TECS _ecs;
		private readonly TConfiguration _configuration;

		private readonly ManualResetEvent _abortSignal = new ManualResetEvent(false);
		private readonly AutoResetEvent _continueSignal = new AutoResetEvent(false);

		private Thread _ecsLoopThread;

		private Queue<ICommand> _commandQueue;

		private readonly object _commandQueueLock = new	object();

		#region constructors

		public ECSRunner(int tickInterval, Sequencer<TECS, TConfiguration> sequencer, TECS ecs, TConfiguration configuration)
		{
			_tickInterval = tickInterval;
			_sequencer = sequencer;
			_ecs = ecs;
			_configuration = configuration;
			// TODO: determine initial command queue size based on something other than guesswork
			_commandQueue = new Queue<ICommand>(100);
		}

		#endregion

		#region thread management

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

		#endregion

		private void ECSLoop(object state)
		{
			while (true)
			{
				var lastLoop = DateTime.Now;
				try
				{
					if (_sequencer?.IsComplete ?? false)
					{
						Stop();
					}
					else
					{
						// TODO: decide whether the sequence tick or the ecs tick comes first
						// ecs tick first necessitates startup actions in the sequence as the first frame OnEnter wont happen until adter the tick has completed
						_sequencer?.Tick(_ecs, _configuration);

						#region command handling
						// TODO: this probably shouldbnt be here

						ICommandSystem commandSystem;
						if (_ecs.TryGetSystem(out commandSystem) == false)
						{
							throw new LifecycleException($"Could not locate command processing system");
						}
						var commandQueue = new List<ICommand>();

						foreach (var command in DequeueCommands())
						{
							if (commandSystem.TryHandleCommand(command))
							{
								commandQueue.Add(command);
							}
							// TODO: log failed command, but dont exception
							//throw new SimulationException($"Unhandled Simulation Command: ${message}");
						}
						// TODO: infer player entity id from photon player, rather than command parameter

						#endregion

						_ecs.Tick();

						OnTick(new Tick()
						{
							CurrentTick = _ecs.CurrentTick,
							CommandQueue = commandQueue.ToArray(),

						});
						WaitHandle.WaitAny(new WaitHandle[] {_continueSignal, _abortSignal});
					}
				}
				catch (Exception ex)
				{
//					throw new LifecycleException("Error in ECS runner loop", ex);
					_abortSignal.Set();
					OnException(ex);
					break;
				}
				var sleep = Math.Max(0, _tickInterval - (int)DateTime.Now.Subtract(lastLoop).TotalMilliseconds);
				if (_abortSignal.WaitOne(sleep))
				{
					break;
				}
			}
		}

		#region command queue

		public void EnqueueCommand(ICommand command)
		{
			lock (_commandQueueLock)
			{
				_commandQueue.Enqueue(command);
			}
		}

		private ICommand[] DequeueCommands()
		{
			lock (_commandQueueLock)
			{
				var commands = new ICommand[_commandQueue.Count];
				_commandQueue.CopyTo(commands, 0);
				_commandQueue.Clear();
				return commands;
			}
		}

		#endregion

		protected virtual void OnTick(Tick tick)
		{
			Tick?.Invoke(tick);
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
