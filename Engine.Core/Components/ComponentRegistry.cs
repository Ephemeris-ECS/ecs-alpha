﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Util;

namespace Engine.Components
{ 
	public class ComponentRegistry : IComponentRegistry
	{
		public static Dictionary<Type, HashSet<Type>> ComponentTypeImplementations { get; }

		private readonly Dictionary<Type, HashSet<Entity>> _componentEntities;

		private readonly List<ComponentMatcherGroup> _matcherGroups;

		private readonly IEntityRegistry _entityRegistry;

		static ComponentRegistry()
		{
			
			// build a dictionary of components by the interfaces they implement
			// this can be static since new components aren't added to the app domain at runtime
			ComponentTypeImplementations = ModuleLoader.GetTypesImplementing<IComponent>()
				.SelectMany(componentType => componentType.GetInterfaces()
					.Select(componentInterface => new { ComponentType = componentType, Interface = componentInterface }))
				.GroupBy(componentTuple => componentTuple.Interface)
				.ToDictionary(k => k.Key, v => new HashSet<Type>(v.Select(componentTuple => componentTuple.ComponentType)));
		}

		public ComponentRegistry(IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
			_matcherGroups = new List<ComponentMatcherGroup>();
			_componentEntities = new Dictionary<Type, HashSet<Entity>>();

		}

		public void AddComponentBinding(Entity entity, IComponent component)
		{
			var componentType = component.GetType();
			AddComponentEntityMapping(entity, componentType);

			//caching these in two different ways is sub optimal but I havent decided which I prefer for now!

			var componentInterfaces = ComponentTypeImplementations[componentType];
			foreach (var componentInterface in componentInterfaces)
			{
				AddComponentEntityMapping(entity, componentInterface);
			}

			UpdateMatcherGroups(entity);
		}

		/// <summary>
		/// Add a new component matcher and reevaluate existing entities 
		/// TODO: support removing matchers and matchers that track component changes to living entities
		/// </summary>
		/// <param name="matcher"></param>
		public void RegisterMatcher(ComponentMatcherGroup matcher)
		{
			_matcherGroups.Add(matcher);
			foreach (var entity in _entityRegistry.Entities.Values)
			{
				matcher.TryAddEntity(entity);
			}
		}

		public void UpdateMatcherGroups(Entity entity)
		{
			foreach (var matcherGroup in _matcherGroups)
			{
				matcherGroup.TryAddEntity(entity);
			}
		}

		/// <summary>
		/// This only exists as a post deserialize temporary measure
		/// </summary>
		public void UpdateMatcherGroups()
		{
			foreach (var matcherGroup in _matcherGroups)
			{
				matcherGroup.Clear();
				foreach (var entity in _entityRegistry.Entities.Values)
				{
					matcherGroup.TryAddEntity(entity);
				}
			}
		}

		private void AddComponentEntityMapping(Entity entity, Type componentType)
		{
			HashSet<Entity> componentEntities;

			if (_componentEntities.TryGetValue(componentType, out componentEntities) == false)
			{
				componentEntities = new HashSet<Entity>();
				_componentEntities.Add(componentType, componentEntities);
			}
			componentEntities.Add(entity);
		}

		public void RemoveComponentEntityMapping(Entity entity)
		{
			// clean these up the lazy way - dont even check if the entity has a specific component type
			// TODO: test if going through the entity's components is quicker
			foreach (var componentType in _componentEntities)
			{
				componentType.Value.Remove(entity);
			}
		}

		public IEnumerable<Entity> GetEntitesWithComponent<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			HashSet<Entity> componentEntities;
			if (_componentEntities.TryGetValue(typeof(TComponentInterface), out componentEntities) )
			{
				return componentEntities;
			}

			return new Entity[0];
		}

		public ComponentMatcher CreateMatcher(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter = null)
		{
			var matcher = new ComponentMatcher(componentTypes, entityFilter);
			//RegisterMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup CreateMatcherGroup(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter = null)
		{
			var matcher = new ComponentMatcherGroup(componentTypes, entityFilter);
			RegisterMatcher(matcher);
			return matcher;
		}
	}
}