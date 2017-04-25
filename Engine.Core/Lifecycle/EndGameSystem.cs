using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Systems;

namespace Engine.Lifecycle
{
	// TODO: this is hacky but effective, for now
	public class EndGameSystem : ISystem
	{
		public EndGameState EndGameState { get; private set; }

		public bool TrySetEndGameState(EndGameState endGameState)
		{
			if (EndGameState == EndGameState.Undefined)
			{
				EndGameState = endGameState;
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
