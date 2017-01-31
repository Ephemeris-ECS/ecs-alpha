using System;

namespace Engine.Commands
{
	public interface ICommandHandler
	{
		Type HandlesType { get; }

		bool TryProcessCommand(ICommand command);
	}
}
