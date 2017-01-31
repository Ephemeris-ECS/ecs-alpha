using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Commands
{
	public abstract class CommandHandler<TCommand> : ICommandHandler
		where TCommand : class, ICommand
	{
		public Type HandlesType => typeof(TCommand);

		public bool TryProcessCommand(ICommand command)
		{
			var typedCommand = command as TCommand;
			return typedCommand == null || TryProcessCommand(typedCommand);
		}

		protected abstract bool TryProcessCommand(TCommand command);
	}
}
