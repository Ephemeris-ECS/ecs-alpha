using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;

namespace Engine.Lifecycle.Commands
{
	public class TickCommand : ICommand
	{
	}

	public class TickCommandHandler : CommandHandler<TickCommand>
	{
		private readonly IECS _ecs;



		protected override bool TryProcessCommand(TickCommand command)
		{
			throw new NotImplementedException();
		}
	}
}
