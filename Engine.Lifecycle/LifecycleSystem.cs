using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Systems;

namespace Engine.Lifecycle
{
	public class LifecycleSystem : ISystem
	{
		private EngineState _engineState;

		public event Action<EngineState> StateChanged;

		public event Action<ExitCode> Stopped;

		public LifecycleSystem()
		{
			_engineState = EngineState.NotStarted;
		}

		public bool TryStart()
		{
			switch (_engineState)
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
			_engineState = EngineState.Started;
			OnStateChanged(_engineState);
		}

		public bool TryStop()
		{
			switch (_engineState)
			{
				case EngineState.Started:
					Stop();
					return true;
				default:
					return false;
			}
		}

		private void Stop(ExitCode exitCode = ExitCode.Success)
		{
			_engineState = EngineState.Stopped;
			OnStateChanged(_engineState);
			OnStopped(exitCode);
		}
		
		protected virtual void OnStopped(ExitCode obj)
		{
			Stopped?.Invoke(obj);
		}

		protected virtual void OnStateChanged(EngineState obj)
		{
			StateChanged?.Invoke(obj);
		}
	}
}
