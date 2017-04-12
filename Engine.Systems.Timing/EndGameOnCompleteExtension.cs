using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Lifecycle;

namespace Engine.Systems.Timing
{
	public class EndGameOnCompleteExtension : ITimerExtension
	{
		private readonly EndGameSystem _endGameSystem;

		public EndGameOnCompleteExtension(EndGameSystem endGameSystem)
		{
			_endGameSystem = endGameSystem;
		}

		public void OnComplete()
		{
			_endGameSystem.TrySetEndGameState(EndGameState.Neutral);
		}
	}
}
