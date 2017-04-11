using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Engine.Common;
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

		private readonly Dictionary<Type, CreateComponentBinding> _components;

		public EntityFactory(DiContainer factoryContainer,
			Archetype archetype, 
			IEntityRegistry entityRegistry,
			IMatcherProvider matcherProvider)
		{
			_factoryContainer = factoryContainer;
			Archetype = archetype;
			_entityRegistry = entityRegistry;
			_matcherProvider = matcherProvider;
			_components = new Dictionary<Type, CreateComponentBinding>();
		}

		public void Initialize()
		{
			var inheritance = new SimpleStack<Archetype>();
			var archetype = Archetype;
			inheritance.Push(archetype);
			while (archetype.Ancestor != null)
			{
				archetype = archetype.Ancestor;
				inheritance.Push(archetype);
			}

			while (inheritance.TryPeek(out archetype))
			{
				foreach (var componentBinding in archetype.Components.Values)
				{
					switch (componentBinding)
					{

						case RemoveComponentBinding r:
							_components.Remove(r.ComponentType);
							break;
						case CreateComponentBinding c:
							_components[c.ComponentType] = c;
							break;
						default:
							break;

					}
					
				}

				inheritance.Pop();
			}

			foreach (var componentBinding in _components.Values)
			{
				_factoryContainer.Bind(componentBinding.ComponentType).AsTransient();
			}

			InitialiseTemplates();
		}

		private void InitialiseTemplates()
		{
			foreach (var componentBinding in _components.Values)
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

				foreach (var componentBinding in _components.Values)
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
