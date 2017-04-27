using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;
using Engine.Lifecycle.Events;
using Engine.Systems;

namespace Engine.Lifecycle
{
	// TODO: this is hacky but effective, for now
	public class EndGameSystem : ISystem
	{
		public EndGameState EndGameState { get; private set; }

		private readonly EventSystem _eventSystem;

		public EndGameSystem(EventSystem eventSystem)
		{
			_eventSystem = eventSystem;
		}

		public bool TrySetEndGameState(EndGameState endGameState)
		{
			if (EndGameState == EndGameState.Undefined)
			{
				EndGameState = endGameState;

				var @event = new EndGameEvent()
				{
					State = EndGameState,
				};
				_eventSystem.Publish(@event);

				return true;
			}
			return false;
		}

		public void Dispose()
		{
			// nothing to dispose
		}

	}
}
