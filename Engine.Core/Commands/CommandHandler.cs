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

		public bool TryHandleCommand(ICommand command, int currentTick)
		{
			var typedCommand = command as TCommand;
			return typedCommand == null || TryHandleCommand(typedCommand, currentTick, Enabled);
		}

		// TODO: I dont like passing the enabled flag through this for the purpose of the events
		// perhaps creata base CommandEvent and take a TEvent type parameter in this class with a default Disabled Result Status
		protected abstract bool TryHandleCommand(TCommand command, int currentTick, bool handlerEnabled);

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
