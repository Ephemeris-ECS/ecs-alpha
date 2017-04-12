using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Configuration;
using Zenject;

namespace Engine.Systems.Timing
{
	public class TimerSystem : ITickableSystem
	{
		private int? _tickLimit;

		public TimeSpan Current { get; private set; }

		private readonly ECSConfiguration _configuration;

		private readonly List<ITimerExtension> _extensions;

		public TimerSystem(ECSConfiguration configuration,
			[InjectOptional] List<ITimerExtension> extensions)
		{
			_configuration = configuration;
			_extensions = extensions;
		}

		public void Tick(int currentTick)
		{
			if (_tickLimit.HasValue)
			{
				var remaining = _tickLimit.Value - currentTick;

				if (remaining <= 0)
				{
					OnComplete();
				}
				else
				{
					Current = TimeSpan.FromMilliseconds(remaining * _configuration.LifeCycleConfiguration.TickInterval);
				}
			}
			else
			{
				Current = TimeSpan.Zero;
			}
		}

		private void OnComplete()
		{
			ExecuteExtensionAction(e => e.OnComplete());
			_tickLimit = null;
			Current = TimeSpan.Zero; ;
		}

		private void ExecuteExtensionAction(Action<ITimerExtension> action)
		{
			foreach (var extension in _extensions)
			{
				action(extension);
			}
		}

		public void SetLimit(int seconds)
		{
			_tickLimit = (seconds * 1000) / _configuration.LifeCycleConfiguration.TickInterval;
			Current = TimeSpan.Zero;
		}
	}
}
