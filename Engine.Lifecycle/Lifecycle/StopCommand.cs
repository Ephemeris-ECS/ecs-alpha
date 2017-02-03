using Engine.Commands;

namespace Engine.Lifecycle.Lifecycle
{
	public class StopCommand : ICommand
	{
	}

	public class StopCommandHandler : CommandHandler<StopCommand>
	{
		private readonly LifecycleSystem _lifecycleSystem;

		public StopCommandHandler(LifecycleSystem lifecycleSystem)
		{
			_lifecycleSystem = lifecycleSystem;
		}

		protected override bool TryProcessCommand(StopCommand command)
		{
			return _lifecycleSystem.TryStop();
		}
	}
}
