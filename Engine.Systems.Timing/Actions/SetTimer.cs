using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Sequencing;
using Engine.Systems.Timing.Commands;

namespace Engine.Systems.Timing.Actions
{
	public class SetTimer<TECS, TConfiguration> : ECSAction<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public SetTimer(int seconds)
		{
			Action = (ecs, config) =>
			{
				var createNpcCommand = new SetTimerCommand()
				{
					Seconds = seconds,
				};
				ecs.EnqueueCommand(createNpcCommand);
			};
		}
	}
}
