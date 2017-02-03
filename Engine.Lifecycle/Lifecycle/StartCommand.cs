using Engine.Commands;

namespace Engine.Lifecycle.Lifecycle
{
	public class StartCommand : ICommand
	{
	}

	public class StartCommandHandler : CommandHandler<StartCommand>
	{
		private readonly LifecycleSystem _lifecycleSystem;

		public StartCommandHandler(LifecycleSystem lifecycleSystem)
		{
			_lifecycleSystem = lifecycleSystem;
		}

		protected override bool TryProcessCommand(StartCommand command)
		{
			return _lifecycleSystem.TryStart();
		}
	}
}
