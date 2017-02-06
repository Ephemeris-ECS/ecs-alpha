using System;

namespace Engine.Commands
{
	public class HaltAndCatchFireCommand : ICommand
	{
	}

	public class HaltAndCatchFireCommandHandler : CommandHandler<HaltAndCatchFireCommand>
	{
		protected override bool TryProcessCommand(HaltAndCatchFireCommand command)
		{
			throw new HaltAndCatchFireException();
		}
	}

	public class HaltAndCatchFireException : Exception
	{
	}
}
