using Engine.Commands;

namespace Engine.Lifecycle.Commands
{
	public class StartCommand : ICommand
	{
	}

	public class StartCommandHandler : CommandHandler<StartCommand>
	{
		private readonly ILifecycleManager _lifecycleManager;

		public StartCommandHandler(ILifecycleManager lifecycleManager)
		{
			_lifecycleManager = lifecycleManager;
		}

		protected override bool TryProcessCommand(StartCommand command)
		{
			return _lifecycleManager.TryStart();
		}
	}
}
