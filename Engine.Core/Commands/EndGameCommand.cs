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

		#region Equality members

		protected bool Equals(EndGameCommand other)
		{
			return EndGameState == other.EndGameState;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EndGameCommand) obj);
		}

		public override int GetHashCode()
		{
			return (int) EndGameState;
		}

		#region Implementation of IEquatable<ICommand>

		public bool Equals(ICommand other)
		{
			return Equals(other as EndGameCommand);
		}

		#endregion

		#endregion
	}

	public class EndGameCommandHandler : CommandHandler<EndGameCommand>
	{
		private EndGameSystem _endGameSystem;

		public EndGameCommandHandler(EndGameSystem endGameSystem)
		{
			_endGameSystem = endGameSystem;
		}

		protected override bool TryProcessCommand(EndGameCommand command, int currentTick)
		{
			return _endGameSystem.TrySetEndGameState(command.EndGameState);
		}
	}
}
