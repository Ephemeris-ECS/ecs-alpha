using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Zenject;

// ReSharper disable InconsistentNaming

namespace Engine.Sequencing
{
	public class ECSAction<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public Action<TECS, TConfiguration> Action { get; private set; }

		public string Name { get; private set; }

		[Inject]
		public ECSAction()
		{
		}

		public ECSAction(Action<TECS, TConfiguration> action, string name)
		{
			Action = action;
			Name = name;
		}

		public virtual void Execute(TECS ecs, TConfiguration configuration)
		{
			Action(ecs, configuration);
		}
	}
}
