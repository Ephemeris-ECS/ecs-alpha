using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Engine.Entities;

namespace Engine.Components
{
	public interface IMatcherProvider
	{
		void UpdateMatchersForEntity(Entity entity);
		void RegisterMatcher(ComponentMatcherGroup matcher);
		
		#region matcher factory

		ComponentMatcher CreateMatcher(Type[] componentTypes, Predicate<Entity> entityFilter = null);
		ComponentMatcherGroup CreateMatcherGroup(Type[] componentTypes, Predicate<Entity> entityFilter = null);

		ComponentMatcherGroup<TComponent1> CreateMatcherGroup<TComponent1>(Predicate<Entity> entityFilter = null)
			where TComponent1 : class, IComponent;

		ComponentMatcherGroup<TComponent1, TComponent2> CreateMatcherGroup<TComponent1, TComponent2>(Predicate<Entity> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent;

		ComponentMatcherGroup<TComponent1, TComponent2, TComponent3> CreateMatcherGroup<TComponent1, TComponent2, TComponent3>(Predicate<Entity> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
			where TComponent3 : class, IComponent;

		#endregion
	}
}