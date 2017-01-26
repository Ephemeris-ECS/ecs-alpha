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
		private static readonly JsonSerializer ComponentTemplateSerializer;

		static EntityFactory()
		{
			// TODO: we probably need some settings overrides
			ComponentTemplateSerializer = JsonSerializer.CreateDefault();
		}

		private readonly DiContainer _factoryContainer;

		private readonly IComponentRegistry _componentRegistry;

		public Archetype Archetype { get; }

		private Dictionary<Type, string> _componentTemplates;

		public EntityFactory(DiContainer factoryContainer, Archetype archetype, IComponentRegistry componentRegistry)
		{
			_factoryContainer = factoryContainer;
			Archetype = archetype;
			_componentTemplates = archetype.Components.ToDictionary(k => k.ComponentType, v => v.ComponentTemplateSerialized);
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

				string componentTemplate;
				if (_componentTemplates.TryGetValue(component.GetType(), out componentTemplate))
				{
					// TODO: this is probably far too slow, we need to build a cache and copy
					using (var reader = new JsonTextReader(new StringReader(componentTemplate)))
					{
						ComponentTemplateSerializer.Populate(reader, component);
					}
				}
			}

			return entity;
		}
	}
}
