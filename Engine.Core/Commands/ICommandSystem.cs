using Engine.Systems;

namespace Engine.Commands
{
	public interface ICommandSystem : ISystem
	{
		bool TryHandleCommand(ICommand command);
		bool TryGetHandler<TCommand>(out ICommandHandler commandHandler) where TCommand : ICommand;
	}
}