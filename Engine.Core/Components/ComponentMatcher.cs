using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Engine.Entities;
using Engine.Exceptions;

namespace Engine.Components
{
	public abstract class ComponentMatcher
	{
		protected int[][] ComponentTypeIds { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentTypes">Entities containing all component types will be matched</param>
		/// <param name="entityFilter">Additional predicate filter to reduce matching entities</param>
		internal ComponentMatcher(Type[] componentTypes, Predicate<Entity> entityFilter = null)
		{
			ComponentTypeIds = componentTypes.Select(ct =>
			{
				if ((ct.IsInterface || ct.IsAbstract) == false)
				{
					return new[] {MatcherProvider.ComponentTypeMap[ct]};
				}
				if (MatcherProvider.ComponentTypesByImplementation.ContainsKey(ct))
				{
					return MatcherProvider.ComponentTypesByImplementation[ct].ToArray();
				}
				return new int[0];
			}).ToArray();

			// relatively expensive test but we need it for debugging
			if (componentTypes.Any(ct => MatcherProvider.ComponentTypeMap.ContainsKey(ct) || MatcherProvider.ComponentTypesByImplementation.ContainsKey(ct)) == false)
			{
				throw new EngineException($"Component type(s) not found in implementation cache {componentTypes.Aggregate(new StringBuilder(), (sb, ct) => sb.Append($"{ct.Name}, "), sb => sb.ToString())})");
			}
		}

		protected bool IsTypeMatch(Entity entity)
		{
			// TODO: this doesnt Support component interfaces
			// TODO: this can probably be much more efficient when being called by the generic subtypes
			return ComponentTypeIds.All(type => type.Any(impl => entity.Components[impl] != null));
		}
	}
}
