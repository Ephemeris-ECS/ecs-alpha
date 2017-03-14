using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Engine.Commands;
using Engine.Configuration;
using Engine.Sequencing;
using Engine.Startup;
using Zenject;


namespace Engine.Lifecycle
{
	public delegate void LifecycleTick(Tick tick, uint crc);
	
	// ReSharper disable InconsistentNaming
	// TODO: this class is a good candidate for merging with ECS runner because there are so many methods on there that do nothing but proxy to the runner
	public abstract class LifecycleManager<TECS, TConfiguration, TInstaller, TECSRoot> : ILifecycleManager, IDisposable
		where TECS : ECS<TConfiguration>
		where TConfiguration : ECSConfiguration
		where TInstaller : ECSInstaller<TECS, TConfiguration, TInstaller, TECSRoot>
		where TECSRoot : ECSRoot<TECS, TConfiguration>
	{
		#region events

		public event Action<EngineState> StateChanged;

		public event Action<ExitCode> Stopped;

		public event LifecycleTick Tick;

		public event Action<Exception> Exception;
		
		//private event ECSTick ECSTick;
		
		#endregion
		public EngineState EngineState { get; protected set; }

		public TECSRoot ECSRoot { get; private set; }

		private readonly Sequencer<TECS, TConfiguration> _sequencer;

		private bool _disposed;

		#region thread members
		
		
		private readonly ManualResetEvent _abortSignal = new ManualResetEvent(false);
		private readonly AutoResetEvent _continueSignal = new AutoResetEvent(false);

		private Thread _ecsLoopThread;


		#endregion

		protected LifecycleManager([InjectOptional] Sequencer<TECS, TConfiguration> sequencer)
		{
			_sequencer = sequencer;
		}

		#region static initializers

		public static TLifecycleManager Initialize<TLifecycleManager, TScenario>(TScenario scenario)
			where TLifecycleManager : LifecycleManager<TECS, TConfiguration, TInstaller, TECSRoot>
			where TScenario : Scenario<TECS, TConfiguration>
		{
			//TODO: too much of this method body is duplicated in the overload

			var container = new DiContainer();

			var sequencer = new Sequencer<TECS, TConfiguration>(scenario);
			container.BindInstance(sequencer);

			container.Bind<ILifecycleManager>().To<TLifecycleManager>().AsSingle();
			var lifecycleManager = container.Instantiate<TLifecycleManager>();

			container.Bind<ECSConfiguration>().FromInstance(scenario.Configuration);
			container.BindInstance(scenario.Configuration);
			container.Instantiate<TInstaller>().InstallBindings();

			var ecsRoot = container.Instantiate<TECSRoot>();
			lifecycleManager.ECSRoot = ecsRoot;

			return lifecycleManager;
		}
		
		public static TLifecycleManager Initialize<TLifecycleManager>(TConfiguration configuration)
			where TLifecycleManager : LifecycleManager<TECS, TConfiguration, TInstaller, TECSRoot>
		{
			var container = new DiContainer();

			container.Bind<ILifecycleManager>().To<TLifecycleManager>().AsSingle();
			var lifecycleManager = container.Instantiate<TLifecycleManager>();

			container.Bind<ECSConfiguration>().FromInstance(configuration);
			container.BindInstance(configuration);
			container.Instantiate<TInstaller>().InstallBindings();
			var ecsRoot = container.Instantiate<TECSRoot>();

			lifecycleManager.ECSRoot = ecsRoot;

			return lifecycleManager;
		}

		#endregion

		public void EnqueueCommand(ICommand command)
		{
			ECSRoot.ECS.EnqueueCommand(command);
		}


		#region thread management

		public void StartThread()
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

		public void StopThread()
		{
			_abortSignal.Set();
			if (Thread.CurrentThread != _ecsLoopThread)
			{
				_ecsLoopThread.Join(10000);
			}
		}

		private void ECSLoop(object state)
		{
			while (true)
			{
				var lastLoop = DateTime.Now;
				try
				{
					if (_sequencer?.IsComplete ?? false)
					{
						StopThread();
						OnSequenceComplete();
					}
					else
					{
						// TODO: decide whether the sequence tick or the ecs tick comes first
						// ecs tick first necessitates startup actions in the sequence as the first frame OnEnter wont happen until adter the tick has completed
						_sequencer?.Tick(ECSRoot.ECS, ECSRoot.Configuration);

						var tick = ECSRoot.ECS.Tick();
						OnTick(tick);

						WaitHandle.WaitAny(new WaitHandle[] { _continueSignal, _abortSignal });
					}
				}
				catch (Exception ex)
				{
					// throw new LifecycleException("Error in ECS runner loop", ex);
					_abortSignal.Set();
					OnException(ex);
					break;
				}
				var sleep = Math.Max(0, ECSRoot.Configuration.LifeCycleConfiguration.TickInterval - (int)DateTime.Now.Subtract(lastLoop).TotalMilliseconds);
				if (_abortSignal.WaitOne(sleep))
				{
					break;
				}
			}
		}

		protected virtual void OnTick(Tick tick)
		{
			_continueSignal.Set();

			// TODO: this shopuld be pushed down into the runner but currently it doesnt have a reference to the root with its serializers so it can remain here on the event handler for now
			uint crc;
			ECSRoot.GetEntityState(out crc);
			//System.IO.File.WriteAllText($"d:\\temp\\{ECSRoot.ECS.CurrentTick}.server.json", state);

			Tick?.Invoke(tick, crc);
		}

		protected virtual void OnSequenceComplete()
		{
			StopInternal(ExitCode.Complete);
		}

		#endregion

		#region external management

		private void SetState(EngineState state)
		{
			if (state != EngineState)
			{
				EngineState = state;
				OnStateChanged();
			}
		}

		public bool TryStart()
		{
			switch (EngineState)
			{
				case EngineState.NotStarted:
				case EngineState.Paused:
					Start();
					return true;
				default:
					return false;
			}
		}

		private void Start()
		{
			// TODO: verify not already running
			StartThread();
			SetState(EngineState.Started);
		}

		public bool TryStop()
		{
			switch (EngineState)
			{
				case EngineState.Started:
				case EngineState.Paused:
					StopInternal();
					return true;
				default:
					return false;
			}
		}
		private void StopInternal(ExitCode exitCode = ExitCode.Abort)
		{
			StopThread();
			SetState(EngineState.Stopped);
			OnStopped(exitCode);
		}

		public bool TryPause()
		{
			switch (EngineState)
			{
				case EngineState.Started:
					PauseInternal();
					return true;
				default:
					return false;
			}
		}
		
		private void PauseInternal()
		{
			SetState(EngineState.Paused);
		}

		protected virtual void OnStopped(ExitCode obj)
		{
			Stopped?.Invoke(obj);
		}

		#endregion

		#region public event invocation

		protected virtual void OnStateChanged()
		{
			StateChanged?.Invoke(EngineState);
		}


		protected virtual void OnException(Exception obj)
		{
			StopInternal(ExitCode.Error);

			Exception?.Invoke(obj);
		}

		#endregion

		public void Dispose()
		{
			if (_disposed == false)
			{
				_disposed = true;
				StopThread();
				((IDisposable)_abortSignal)?.Dispose();
				((IDisposable)_continueSignal)?.Dispose();
			}
		}
	}
}
