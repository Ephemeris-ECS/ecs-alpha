using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Sequencing;
using Engine.Startup;
using Engine.Systems;
using Zenject;
// ReSharper disable InconsistentNaming

namespace Engine.Lifecycle
{
	public abstract class LifecycleManager<TECS, TConfiguration, TInstaller, TECSRoot> : ILifecycleManager, IDisposable
		where TECS : ECS<TConfiguration>
		where TConfiguration : ECSConfiguration
		where TInstaller : ECSInstaller<TECS, TConfiguration, TInstaller, TECSRoot>
		where TECSRoot : ECSRoot<TECS, TConfiguration>
	{
		public EngineState EngineState { get; protected set; }

		public event Action<EngineState> StateChanged;

		public event Action<ExitCode> Stopped;

		public event Action Tick;

		public event Action<Exception> Exception;

		public TECSRoot ECSRoot { get; private set; }

		protected Sequencer<TECS, TConfiguration> Sequencer { get; set; }

		private ECSRunner<TECS, TConfiguration> _runner;

		private bool _disposed = false;

		protected LifecycleManager([InjectOptional] Sequencer<TECS, TConfiguration> sequencer)
		{
			Sequencer = sequencer;
		}

		#region static initializers

		public static TLifecycleManager Initialize<TLifecycleManager, TScenario>(TScenario scenario)
			where TLifecycleManager : LifecycleManager<TECS, TConfiguration, TInstaller, TECSRoot>
			where TScenario : Scenario<TECS, TConfiguration>
		{
			var container = new DiContainer();

			var sequencer = new Sequencer<TECS, TConfiguration>(scenario);
			container.BindInstance(sequencer);

			container.Bind<ILifecycleManager>().To<TLifecycleManager>().AsSingle();
			var lifecycleManager = container.Instantiate<TLifecycleManager>();

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

			container.BindInstance(configuration);
			container.Instantiate<TInstaller>().InstallBindings();
			var ecsRoot = container.Instantiate<TECSRoot>();

			lifecycleManager.ECSRoot = ecsRoot;

			return lifecycleManager;
		}

		#endregion

		private void InitializeRunner()
		{
			if (_runner == null)
			{
				_runner = new ECSRunner<TECS, TConfiguration>(ECSRoot.Configuration.LifeCycleConfiguration.TickInterval, Sequencer, ECSRoot.ECS, ECSRoot.Configuration);
				_runner.Tick += OnTick;
				_runner.Exception += RunnerOnException;
			}
		}

		private void RunnerOnException(Exception exception)
		{
			StopInternal(ExitCode.Error);
			OnException(exception);
		}

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
			InitializeRunner();
			_runner.Start();
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
		private void StopInternal(ExitCode exitCode = ExitCode.Success)
		{
			_runner?.Stop();
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

		public bool TryTick()
		{
			switch (EngineState)
			{
				case EngineState.NotStarted:
				case EngineState.Paused:
					Tick();
					return true;
				default:
					return false;
			}
		}

		private void TickInternal()
		{
			ECSRoot.ECS.Tick();
			Sequencer?.Tick(ECSRoot.ECS, ECSRoot.Configuration);
			if (Sequencer?.IsComplete ?? false)
			{
				// TODO: need the sequence to evaluate the exit code
				StopInternal(ExitCode.Success);
			}
		}

		protected virtual void OnStopped(ExitCode obj)
		{
			Stopped?.Invoke(obj);
		}

		protected virtual void OnStateChanged()
		{
			StateChanged?.Invoke(EngineState);
		}

		protected virtual void OnTick()
		{
			Tick?.Invoke();
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_runner?.Dispose();
			}
		}

		protected virtual void OnException(Exception obj)
		{
			Exception?.Invoke(obj);
		}
	}
}
