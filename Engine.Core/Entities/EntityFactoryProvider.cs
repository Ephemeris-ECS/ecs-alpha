using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Exceptions;
using Zenject;

namespace Engine.Entities
{
	public class EntityFactoryProvider : IEntityFactoryProvider
	{
		private IEntityRegistry _entityRegistry;

		private readonly Dictionary<string, IEntityFactory> _entityFactories;

		internal EntityFactoryProvider(IEntityRegistry entityRegistry, 
			[InjectOptional] List<IEntityFactory> entityFactories)
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
		{
			_entityRegistry = entityRegistry;
			_entityFactories = entityFactories.ToDictionary(k => k.Archetype.Name, v => v);
		}

		public void InitializeFactories()
		{
			foreach (var factory in _entityFactories)
			{
				try
				{
					factory.Value.Initialize();
				}
				catch (Exception ex)
				{
					throw new EngineException($"Error initializing entity factory for Archetype {factory.Value.Archetype.Name}: ex");
				}
			}
		}

		public bool TryCreateEntityFromArchetype(string archetypeName, out Entity entity)
		{
			IEntityFactory entityFactory;
			if (_entityFactories.TryGetValue(archetypeName, out entityFactory))
			{
				entity = entityFactory.CreateEntityFromArchetype();
				return true;
			}
			entity = null;
			return false;
		}
	}
}
