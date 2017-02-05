using Engine.Systems;

namespace Engine.Commands
{
	public interface ICommandSystem : ISystem
	{
		bool TryHandleCommand(ICommand command);
	}
}