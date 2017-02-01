using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Engine.Entities;
using Engine.Exceptions;

namespace Engine.Components
{
	public class ComponentMatcher
	{
		public HashSet<int> ComponentTypeIds { get; }

		private Predicate<Entity> EntityFilter { get; }

		protected internal ComponentMatcher()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentTypes">Entities containing all component types will be matched</param>
		/// <param name="entityFilter">Additional predicate filter to reduce matching entities</param>
		internal ComponentMatcher(Type[] componentTypes, Predicate<Entity> entityFilter = null)
		{
			ComponentTypeIds = new HashSet<int>(componentTypes
				.Where(ct => (ct.IsInterface || ct.IsAbstract) == false)
				.Select(ct => ComponentRegistry.ComponentTypeMap[ct])
				.Union(componentTypes
					.Where(ct => ComponentRegistry.ComponentTypesByImplementation.ContainsKey(ct))
					.SelectMany(ct => ComponentRegistry.ComponentTypesByImplementation[ct])));

			// relatively expensive test but we need it for debugging
			if (componentTypes.Any(ct => ComponentRegistry.ComponentTypeMap.ContainsKey(ct) || ComponentRegistry.ComponentTypesByImplementation.ContainsKey(ct)) == false)
			{
				throw new EngineException($"Component type(s) not found in implementation cache {componentTypes.Aggregate(new StringBuilder(), (sb, ct) => sb.Append($"{ct.Name}, "), sb => sb.ToString())})");
			}

			EntityFilter = entityFilter ?? (entity => true);
		}

		public virtual bool IsMatch(Entity entity)
		{
			// TODO: this doesnt Support component interfaces
			// TODO: this can probably be much more efficient when being called by the generic subtypes
			return ComponentTypeIds.All(rt => entity.Components[rt] != null) && EntityFilter(entity);
		}
	}
}
