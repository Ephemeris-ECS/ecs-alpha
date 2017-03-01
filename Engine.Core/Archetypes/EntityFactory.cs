using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Newtonsoft.Json;
using Zenject;

namespace Engine.Archetypes
{
	public class EntityFactory : IEntityFactory
	{
		private readonly DiContainer _factoryContainer;

		private readonly IEntityRegistry _entityRegistry;

		private readonly IMatcherProvider _matcherProvider;

		public Archetype Archetype { get; }

		public EntityFactory(DiContainer factoryContainer,
			Archetype archetype, 
			IEntityRegistry entityRegistry,
			IMatcherProvider matcherProvider)
		{
			_factoryContainer = factoryContainer;
			Archetype = archetype;
			_entityRegistry = entityRegistry;
			_matcherProvider = matcherProvider;

			InitialiseTemplates();
		}

		private void InitialiseTemplates()
		{
			foreach (var componentBinding in Archetype.Components.Values)
			{
				try
				{
					componentBinding.InitialiseTemplate();
				}
				catch (Exception ex)
				{
					throw new EntityFactoryException($"Error initialising component for archetype '{Archetype.Name}', component type {componentBinding.ComponentType.Name}", ex);
				}
			}
		}

		public Entity CreateEntityFromArchetype()
		{
			Entity entity = null;
			try
			{
				entity = _entityRegistry.CreateEntity();

				foreach (var componentBinding in Archetype.Components.Values)
				{
					try
					{
						var component = (IComponent) _factoryContainer.Resolve(componentBinding.ComponentType);
						componentBinding.PopulateComponent(component);
						entity.AddComponent(component);
					}
					catch (Exception ex)
					{
						throw new EntityFactoryException($"Error initialising component for archetype '{Archetype.Name}', component type {componentBinding.ComponentType.Name}", ex);

					}
				}
				_matcherProvider.UpdateMatchersForEntity(entity);


				return entity;
			}
			catch (Exception ex)
			{
				entity?.Dispose();
				throw new EntityFactoryException($"Error creating entity from archetype '{Archetype.Name}'", ex);
			}
		}
	}
}
