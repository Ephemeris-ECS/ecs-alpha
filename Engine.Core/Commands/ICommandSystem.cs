using System;
using Engine.Systems;

namespace Engine.Commands
{
	public interface ICommandSystem : ISystem
	{
		bool TryHandleCommand(ICommand command);

		bool TryGetHandler(Type commandType, out ICommandHandler commandHandler);

		bool TryGetHandler<TCommand>(out ICommandHandler commandHandler) where TCommand : ICommand;
	}
}