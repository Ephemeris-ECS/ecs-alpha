using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;
using Zenject;

//using System.Reactive.Disposables;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECS<TConfiguration> : IDisposable, IECS<TConfiguration>
		where TConfiguration : ECSConfiguration
	{
		private bool _disposed;

		private int _tick;

		public TConfiguration Configuration { get; }

		/// <summary>
		/// This is where the entity pool lives and new entities are created
		/// </summary>
		public IEntityRegistry EntityRegistry { get; private set; }

		/// <summary>
		/// This is where the component pool lives and component to entity mappings take place
		/// </summary>
		public IComponentRegistry ComponentRegistry { get; private set; }

		/// <summary>
		/// This is where the system pool lives and systems are activated 
		/// </summary>
		public ISystemRegistry SystemRegistry { get; private set; }

		/// <summary>
		/// This factory creates components and popualtes an entity when an archetype is instantiated
		/// TODO: refactor this into the component registry
		/// </summary>
		protected ComponentFactory ComponentFactory { get; private set; }

		/// <summary>
		/// Dicyionary of entity archetypes
		/// TODO: allow this to be modified at runtim and/or loaded from configuration
		/// </summary>
		protected Dictionary<string, IEntityFactory> Archetypes { get; private set; }

		protected ECS(TConfiguration configuration,
			IEntityRegistry entityRegistry,
			IComponentRegistry componentRegistry,
			ISystemRegistry systemRegistry,
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			[InjectOptional] List<IEntityFactory> entityFactories)
		{
			Configuration = configuration;
			EntityRegistry = entityRegistry;
			ComponentRegistry = componentRegistry;
			SystemRegistry = systemRegistry;
			ComponentFactory = new ComponentFactory();
			// signal the component registry that a new entity has been populated
			ComponentFactory.EntityArchetypeCreated += ComponentRegistry.UpdateMatcherGroups;

			Archetypes = entityFactories.ToDictionary(k => k.Archetype.Name, v => v);
		}

		public Entity CreateEntityFromArchetype(string archetypeName)
		{
			IEntityFactory entityFactory;
			if (Archetypes.TryGetValue(archetypeName, out entityFactory))
			{
				return entityFactory.CreateEntity();
			}
			throw new KeyNotFoundException($"No archetype found for key '{archetypeName}'");
		}

		public Dictionary<int, Entity> GetEntities()
		{
			return EntityRegistry.Entities;
		}

		public void Dispose()
		{
			if (_disposed == false)
			{
				
			}
		}

		public void Tick()
		{
			SystemRegistry.Tick(++_tick);
		}

	}
}
