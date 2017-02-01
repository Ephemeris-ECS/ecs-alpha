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
		protected IEntityRegistry EntityRegistry { get; private set; }

		public EntityDictionary Entities => EntityRegistry.Entities;


		/// <summary>
		/// This is where the component pool lives and component to entity mappings take place
		/// </summary>
		protected IComponentRegistry ComponentRegistry { get; private set; }

		/// <summary>
		/// This is where the system pool lives and systems are activated 
		/// </summary>
		protected ISystemRegistry SystemRegistry { get; private set; }

		/// <summary>
		/// This factory creates components and popualtes an entity when an archetype is instantiated
		/// TODO: refactor this into the component registry
		/// </summary>
		protected ComponentFactory ComponentFactory { get; private set; }

		protected EntityFactoryProvider EntityFactoryProvider { get; }

		protected ECS(TConfiguration configuration,
			IEntityRegistry entityRegistry,
			IComponentRegistry componentRegistry,
			ISystemRegistry systemRegistry,
			EntityFactoryProvider entityFactoryProvider
			
			)
		{
			Configuration = configuration;
			EntityRegistry = entityRegistry;
			ComponentRegistry = componentRegistry;
			SystemRegistry = systemRegistry;
			ComponentFactory = new ComponentFactory();
			// signal the component registry that a new entity has been populated
			ComponentFactory.EntityArchetypeCreated += ComponentRegistry.UpdateMatcherGroups;
			EntityFactoryProvider = entityFactoryProvider;
		}

		/// <summary>
		/// Perform initialization operations,
		/// this includes ISystemRegistry.Initialize which activates all of the IInitializingSystem(s)
		/// </summary>
		public virtual void Initialize()
		{
			SystemRegistry.Initialize();
		}

		#region System access

		public bool TryGetSystem<TSystem>(out TSystem system)
			where TSystem : class, ISystem
		{
			return SystemRegistry.TryGetSystem<TSystem>(out system);
		}

		public IEnumerable<TSystem> GetSystems<TSystem>()
			where TSystem : class, ISystem
		{
			return SystemRegistry.GetSystems<TSystem>();
		}

		public bool TryCreateEntityFromArchetype(string archetype, out Entity entity)
		{
			return EntityFactoryProvider.TryCreateEntityFromArchetype(archetype, out entity);
		}

		#endregion

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
