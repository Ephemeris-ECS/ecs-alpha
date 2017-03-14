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

		// TODO: this should be stored on an entity, or transferred to the slave client somehow
		public bool Enabled { get; protected set; } = true;

		public virtual void SetEnabled(bool enabled)
		{
			Enabled = enabled;
		}

		#region Implementation of ICommandHandler

		public virtual IEqualityComparer<ICommand> Deduplicator => CommandEqualityComparer.Default;

		#endregion
	}
}
