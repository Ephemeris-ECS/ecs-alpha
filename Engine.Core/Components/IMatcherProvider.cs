using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using Engine.Entities;

namespace Engine.Components
{
	public interface IMatcherProvider
	{
		void UpdateMatchersForEntity(Entity entity);
		
		#region matcher factory

		ComponentMatcherGroup<TComponent1> CreateMatcherGroup<TComponent1>(Predicate<ComponentEntityTuple<TComponent1>> entityFilter = null)
			where TComponent1 : class, IComponent;

		ComponentMatcherGroup<TComponent1, TComponent2> CreateMatcherGroup<TComponent1, TComponent2>(Predicate<ComponentEntityTuple<TComponent1, TComponent2>> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent;

		ComponentMatcherGroup<TComponent1, TComponent2, TComponent3> CreateMatcherGroup<TComponent1, TComponent2, TComponent3>(Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
			where TComponent3 : class, IComponent;

		ComponentMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4> CreateMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4>(Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>> entityFilter = null)
			where TComponent1 : class, IComponent
			where TComponent2 : class, IComponent
			where TComponent3 : class, IComponent
			where TComponent4 : class, IComponent;

		#endregion
	}
}