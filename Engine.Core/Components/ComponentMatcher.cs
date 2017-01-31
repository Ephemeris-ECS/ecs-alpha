using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public class ComponentMatcher
	{
		public HashSet<Type> ComponentTypes { get; }

		private Predicate<Entity> EntityFilter { get; }

		protected internal ComponentMatcher()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="componentTypes">Entities containing all component types will be matched</param>
		/// <param name="entityFilter">Additional predicate filter to reduce matching entities</param>
		internal ComponentMatcher(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter = null)
		{
			ComponentTypes = new HashSet<Type>(componentTypes);
			EntityFilter = entityFilter ?? (entity => true);
		}

		public virtual bool IsMatch(Entity entity)
		{
			// TODO: this can probably be much more efficient when being called by the generic subtypes
			return ComponentTypes.All(rt => entity.Components.ContainsKey(rt)) && EntityFilter(entity);
		}
	}
}
