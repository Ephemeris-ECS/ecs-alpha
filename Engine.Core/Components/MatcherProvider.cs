using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Exceptions;
using Engine.Util;

namespace Engine.Components
{ 
	public class MatcherProvider : IMatcherProvider
	{
		public static Dictionary<Type, HashSet<int>> ComponentTypesByImplementation { get; }

		public static Dictionary<Type, int> ComponentTypeMap { get; }

		private readonly Dictionary<Type, HashSet<Entity>> _componentEntities;

		private readonly List<ComponentMatcherGroup> _matcherGroups;

		private readonly IEntityRegistry _entityRegistry;

		static MatcherProvider()
		{
			// TODO: this should probably not use the app domain but the DiContainer to see what has been bound as we dont care about anything 
			// else until components and/or archetypes can be added at runtime

			try
			{
				ComponentTypeMap = ModuleLoader.GetTypesImplementing<IComponent>()
				.Select((t, i) => new {Type = t, Index = i})
				.ToDictionary(k => k.Type, v => v.Index);

				// build a dictionary of components by the interfaces they implement
				// this can be static since new components aren't added to the app domain at runtime
				ComponentTypesByImplementation = ModuleLoader.GetTypesImplementing<IComponent>()
					.SelectMany(componentType => componentType.GetInterfaces()
						.Select(componentInterface => new { ComponentType = componentType, Interface = componentInterface }))
					.GroupBy(componentTuple => componentTuple.Interface)
					.ToDictionary(k => k.Key, v => new HashSet<int>(v.Select(componentTuple => ComponentTypeMap[componentTuple.ComponentType])));
			}
			catch (Exception ex)
			{
				throw new EngineException($"Error initializing component type map", ex);
			}
		}

		public MatcherProvider(IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
			_matcherGroups = new List<ComponentMatcherGroup>();
			_componentEntities = new Dictionary<Type, HashSet<Entity>>();

		}
		
		public void UpdateMatchersForEntity(Entity entity)
		{
			foreach (var matcherGroup in _matcherGroups)
			{
				matcherGroup.TryAddEntity(entity);
			}
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

		#region matcher factory

		// TODO: store the component type sets for requested matchers and returned from cache when the same matcher already exists
		// TODO: perhaps build matcher proxy for <currently unused> matcher filter predicate
		// TODO: cache the MatchingEntities array to rpevent repeated projection to array when values have not changed
		//			store a dirty flag or simialr inside matcher and set only when matches have been modified - reduce uncessary testing significantly

		public ComponentMatcher CreateMatcher(Type[] componentTypes, Predicate<Entity> entityFilter = null)
		{
			var matcher = new ComponentMatcher(componentTypes, entityFilter);
			//RegisterMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup CreateMatcherGroup(Type[] componentTypes, Predicate<Entity> entityFilter = null)
		{
			var matcher = new ComponentMatcherGroup(componentTypes, entityFilter);
			RegisterMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup<TComponent1> CreateMatcherGroup<TComponent1>(Predicate<Entity> entityFilter = null)
			where TComponent1 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1>(entityFilter);
			RegisterMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup<TComponent1, TComponent2> CreateMatcherGroup<TComponent1, TComponent2>(Predicate<Entity> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1, TComponent2>(entityFilter);
			RegisterMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup<TComponent1, TComponent2, TComponent3> CreateMatcherGroup<TComponent1, TComponent2, TComponent3>(Predicate<Entity> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
			where TComponent3 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1, TComponent2, TComponent3>(entityFilter);
			RegisterMatcher(matcher);
			return matcher;
		}

		#endregion
	}
}
