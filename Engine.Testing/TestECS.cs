using System.Collections.Generic;
using Engine.Commands;
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
			IMatcherProvider matcherProvider, 
			ISystemRegistry systemRegistry,
			EntityFactoryProvider entityFactoryProvider,
			CommandQueue commandQueue)
			: base(configuration, 
				entityRegistry, 
				matcherProvider, 
				systemRegistry,
				entityFactoryProvider,
				commandQueue)
		{
		}

		public new bool TryCreateEntityFromArchetype(string archetype, out Entity entity)
		{
			return base.TryCreateEntityFromArchetype(archetype, out entity);
		}
	}
}