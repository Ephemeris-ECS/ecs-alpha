using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Commands
{
	// TODO: this is probably a tempory pattern
	public abstract class CommandCapability<TCommand> : ICommandCapability
		where TCommand : ICommand
	{
		public Type HandlesType { get; }
		public bool Evalutate()
		{
			throw new NotImplementedException();
		}
	}
}
