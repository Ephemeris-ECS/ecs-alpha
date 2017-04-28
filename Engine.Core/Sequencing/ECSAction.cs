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
		public Action<TECS, TConfiguration> Action { get; protected set; }

		public string Name { get; protected set; }

		[Inject]
		public ECSAction()
		{
			Name = this.GetType().Name;
		}

		public ECSAction(string name)
		{
			Name = name;
		}

		public ECSAction(Action<TECS, TConfiguration> action, string name)
			: this(name)
		{
			Action = action;
		}

		public virtual void Execute(TECS ecs, TConfiguration configuration)
		{
			Action(ecs, configuration);
		}

		public virtual void Initialize(TECS ecs, TConfiguration configuration)
		{
			
		}

	}
}
