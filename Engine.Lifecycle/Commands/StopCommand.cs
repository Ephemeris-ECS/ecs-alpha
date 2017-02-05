using Engine.Commands;

namespace Engine.Lifecycle.Commands
{
	public class StopCommand : ICommand
	{
	}

	public class StopCommandHandler : CommandHandler<StopCommand>
	{
		private readonly ILifecycleManager _lifecycleManager;

		public StopCommandHandler(ILifecycleManager lifecycleManager)
		{
			_lifecycleManager = lifecycleManager;
		}

		protected override bool TryProcessCommand(StopCommand command)
		{
			return _lifecycleManager.TryStop();
		}
	}
}
