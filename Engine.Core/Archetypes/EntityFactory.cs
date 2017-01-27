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

		private readonly IComponentRegistry _componentRegistry;

		public Archetype Archetype { get; }

		public EntityFactory(DiContainer factoryContainer, Archetype archetype, IComponentRegistry componentRegistry)
		{
			_factoryContainer = factoryContainer;
			Archetype = archetype;
			_componentRegistry = componentRegistry;

			InitialiseTemplates();
		}

		private void InitialiseTemplates()
		{
			foreach (var componentBinding in Archetype.Components)
			{
				try
				{
					componentBinding.InitialiseTemplate();
				}
				catch (Exception ex)
				{
					throw new EntityFactoryException($"Error initialising component template for component type {componentBinding.ComponentType.Name}", ex, Archetype.Name);
				}
			}
		}

		public Entity CreateEntity()
		{
			try
			{
				var entity = _factoryContainer.Instantiate<Entity>();

				foreach (var componentBinding in Archetype.Components)
				{
					var component = (IComponent) _factoryContainer.Resolve(componentBinding.ComponentType);
					componentBinding.PopulateComponent(component);
					entity.AddComponent(component);
					_componentRegistry.AddComponentBinding(entity, component);
				}

				return entity;
			}
			catch (Exception ex)
			{
				throw new EntityFactoryException("Error creating entity from archetype.", ex, Archetype.Name);
			}
		}
	}
}
