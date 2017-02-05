using Engine.Commands;

namespace Engine.Lifecycle.Commands
{
	public class PauseCommand : ICommand
	{
	}
	public class PauseCommandHandler : CommandHandler<PauseCommand>
	{
		private readonly ILifecycleManager _lifecycleManager;

		public PauseCommandHandler(ILifecycleManager lifecycleManager)
		{
			_lifecycleManager = lifecycleManager;
		}

		protected override bool TryProcessCommand(PauseCommand command)
		{
			return _lifecycleManager.TryPause();
		}
	}
}
