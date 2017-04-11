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

		private readonly List<ComponentMatcherGroup> _matcherGroups;

		private readonly IEntityRegistry _entityRegistry;

		static MatcherProvider()
		{
			// TODO: this should probably not use the app domain but the DiContainer to see what has been bound as we dont care about anything 
			// else until components and/or archetypes can be added at runtime
			try
			{
				ComponentTypeMap = ModuleLoader.GetTypesImplementing<IComponent>()
					// the ordering here is necessary as unity (mono) seems to reflect types in a different order than the clr when they are in different assemblies
					// TODO: see if this can be more deterministic - do not use moduleloader
					.OrderBy(t => t.FullName)
					.Select((t, i) => new {Type = t, Index = i})
					.ToDictionary(k => k.Type, v => v.Index);

				// build a dictionary of components by the interfaces they implement
				// this can be static since new components aren't added to the app domain at runtime
				ComponentTypesByImplementation = ModuleLoader.GetTypesImplementing<IComponent>()
					.SelectMany(componentType => componentType.GetInterfaces()
						.Select(componentInterface => new { ComponentType = componentType, Interface = componentInterface }))
					.GroupBy(componentTuple => componentTuple.Interface)
					.OrderBy(ci => ci.Key.FullName)
					.ToDictionary(k => k.Key, v => new HashSet<int>(v.Select(componentTuple => ComponentTypeMap[componentTuple.ComponentType])));
			}
			catch (Exception ex)
			{
				throw new EngineException($"Error initializing component type map: {ex}");
			}
		}

		public MatcherProvider(IEntityRegistry entityRegistry)
		{
			_entityRegistry = entityRegistry;
			_matcherGroups = new List<ComponentMatcherGroup>();
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
		private void AddMatcher(ComponentMatcherGroup matcher)
		{
			_matcherGroups.Add(matcher);
			matcher.Disposed += () => RemoveMatcher(matcher);
			foreach (var entity in _entityRegistry.Entities.Values)
			{
				matcher.TryAddEntity(entity);
			}
		}

		private void RemoveMatcher(ComponentMatcherGroup matcher)
		{
			_matcherGroups.Remove(matcher);
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

		public ComponentMatcherGroup<TComponent1> CreateMatcherGroup<TComponent1>(
			Predicate<ComponentEntityTuple<TComponent1>> entityFilter = null)
			where TComponent1 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1>(entityFilter);
			AddMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup<TComponent1, TComponent2> CreateMatcherGroup<TComponent1, TComponent2>(
			Predicate<ComponentEntityTuple<TComponent1, TComponent2>> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1, TComponent2>(entityFilter);
			AddMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup<TComponent1, TComponent2, TComponent3> CreateMatcherGroup<TComponent1, TComponent2, TComponent3>(
			Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
			where TComponent3 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1, TComponent2, TComponent3>(entityFilter);
			AddMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4> CreateMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4>(
			Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
			where TComponent3 : class, IComponent
			where TComponent4 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4>(entityFilter);
			AddMatcher(matcher);
			return matcher;
		}

		public ComponentMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> CreateMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(
			Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>> entityFilter = null) 
			where TComponent1 : class, IComponent 
			where TComponent2 : class, IComponent
			where TComponent3 : class, IComponent 
			where TComponent4 : class, IComponent 
			where TComponent5 : class, IComponent
		{
			var matcher = new ComponentMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(entityFilter);
			AddMatcher(matcher);
			return matcher;
		}

		#endregion
	}
}
