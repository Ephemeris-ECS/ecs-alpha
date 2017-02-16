using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Lifecycle;

namespace Engine.Commands
{
	public class EndGameCommand : ICommand
	{
		public EndGameState EndGameState { get; set; }
	}

	public class EndGameCommandHandler : CommandHandler<EndGameCommand>
	{
		private EndGameSystem _endGameSystem;

		public EndGameCommandHandler(EndGameSystem endGameSystem)
		{
			_endGameSystem = endGameSystem;
		}

		protected override bool TryProcessCommand(EndGameCommand command)
		{
			return _endGameSystem.TrySetEndGameState(command.EndGameState);
		}
	}
}
