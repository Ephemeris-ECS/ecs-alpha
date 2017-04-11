using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public abstract class ComponentEntityTuple : IEquatable<ComponentEntityTuple>
	{
		public Entity Entity { get; }

		protected ComponentEntityTuple(Entity entity)
		{
			Entity = entity;
		}

		public bool Equals(ComponentEntityTuple other)
		{
			return other?.Entity != null
					&& Entity.Id == other.Entity.Id;
		}
	}

	#region 1 tuple

	public class ComponentEntityTuple<TComponent1> : IEquatable<ComponentEntityTuple<TComponent1>>
		where TComponent1 : IComponent
	{
		public Entity Entity { get; }

		public TComponent1 Component1 { get; }

		public ComponentEntityTuple(Entity entity, TComponent1 component1)
		{
			Entity = entity;
			Component1 = component1;
		}

		public bool Equals(ComponentEntityTuple<TComponent1> other)
		{
			return other?.Entity != null
					&& Entity.Id == other.Entity.Id;
		}
	}

	#endregion

	#region 2 tuple

	public class ComponentEntityTuple<TComponent1, TComponent2> : ComponentEntityTuple<TComponent1>,
			IEquatable<ComponentEntityTuple<TComponent1, TComponent2>>
			where TComponent1 : IComponent
			where TComponent2 : IComponent
	{
		public TComponent2 Component2 { get; }

		public ComponentEntityTuple(Entity entity, TComponent1 component1, TComponent2 component2)
			: base(entity, component1)
		{
			Component2 = component2;
		}

		public bool Equals(ComponentEntityTuple<TComponent1, TComponent2> other)
		{
			return base.Equals(other);
		}
	}

	#endregion

	#region 3 tuple

	public class ComponentEntityTuple<TComponent1, TComponent2, TComponent3> : ComponentEntityTuple<TComponent1, TComponent2>,
		IEquatable<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>>
		where TComponent1 : IComponent
		where TComponent2 : IComponent
		where TComponent3 : IComponent
	{
		public TComponent3 Component3 { get; }

		public ComponentEntityTuple(Entity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3)
			: base(entity, component1, component2)
		{
			Component3 = component3;
		}

		public bool Equals(ComponentEntityTuple<TComponent1, TComponent2, TComponent3> other)
		{
			return base.Equals(other);
		}
	}

	#endregion

	#region 4 tuple

	public class ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4> : ComponentEntityTuple<TComponent1, TComponent2, TComponent3>,
		IEquatable<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>>
		where TComponent1 : IComponent
		where TComponent2 : IComponent
		where TComponent3 : IComponent
		where TComponent4 : IComponent
	{
		public TComponent4 Component4 { get; }

		public ComponentEntityTuple(Entity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3, TComponent4 component4)
			: base(entity, component1, component2, component3)
		{
			Component4 = component4;
		}

		public bool Equals(ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4> other)
		{
			return base.Equals(other);
		}
	}

	#endregion

	#region 5 tuple

	public class ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> : ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>,
		IEquatable<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>>
		where TComponent1 : IComponent
		where TComponent2 : IComponent
		where TComponent3 : IComponent
		where TComponent4 : IComponent
		where TComponent5 : IComponent
	{
		public TComponent5 Component5 { get; }

		public ComponentEntityTuple(Entity entity, TComponent1 component1, TComponent2 component2, TComponent3 component3, TComponent4 component4, TComponent5 component5)
			: base(entity, component1, component2, component3, component4)
		{
			Component5 = component5;
		}

		public bool Equals(ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> other)
		{
			return base.Equals(other);
		}
	}

	#endregion
}

