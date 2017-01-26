using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Components;
using Zenject;

namespace Engine.Entities
{
	public class EntityFactory : IEntityFactory
	{
		private readonly DiContainer _factoryContainer;

		private readonly IComponentRegistry _componentRegistry;

		public Archetype Archetype { get; }

		private Dictionary<Type, string> _initializationData;

		public EntityFactory(DiContainer factoryContainer, Archetype archetype, IComponentRegistry componentRegistry)
		{
			_factoryContainer = factoryContainer;
			Archetype = archetype;
			_initializationData = archetype.Components.ToDictionary(k => k.ComponentType, v => v.ComponentTemplateSerialized);
			_componentRegistry = componentRegistry;
		}

		public Entity CreateEntity()
		{
			var entity = _factoryContainer.Instantiate<Entity>();

			var components = _factoryContainer.Resolve<List<IComponent>>();
			foreach (var component in components)
			{
				entity.AddComponent(component);
				_componentRegistry.AddComponentBinding(entity, component);
				// TODO: populate component from template
			}

			return entity;
		}
	}
}
