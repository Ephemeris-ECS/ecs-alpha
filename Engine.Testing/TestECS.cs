using System.Collections.Generic;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;
using Zenject;

namespace Engine.Testing
{
	// ReSharper disable once InconsistentNaming
	public class TestECS : ECS<ECSConfiguration>
	{
		public TestECS(ECSConfiguration configuration, 
			IEntityRegistry entityRegistry, 
			IComponentRegistry componentRegistry, 
			ISystemRegistry systemRegistry,
			EntityFactoryProvider entityFactoryProvider)
			: base(configuration, 
				entityRegistry, 
				componentRegistry, 
				systemRegistry,
				entityFactoryProvider)
		{
		}
	}
}