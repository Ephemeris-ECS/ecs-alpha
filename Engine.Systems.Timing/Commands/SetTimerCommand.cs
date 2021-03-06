﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;

namespace Engine.Systems.Timing.Commands
{
	public class SetTimerCommand : ICommand
	{
		public int Seconds { get; set; }
	}

	public class SetTimerCommandHandler : CommandHandler<SetTimerCommand>
	{
		private TimerSystem _timerSystem;

		public SetTimerCommandHandler(TimerSystem timerSystem)
		{
			_timerSystem = timerSystem;
		}

		protected override bool TryHandleCommand(SetTimerCommand command, int currentTick, bool handlerEnabled)
		{
			if (handlerEnabled)
			{
				_timerSystem.SetLimit(command.Seconds);
				return true;
			}
			return false;
		}
	}
}
