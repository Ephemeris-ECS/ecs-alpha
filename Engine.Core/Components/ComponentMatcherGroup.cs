using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	#region pure match, no result tuple

	public class ComponentMatcherGroup : ComponentMatcher
	{
		public event Action<Entity> MatchingEntityAdded;

		public event Action<Entity> MatchingEntityRemoved;

		public HashSet<Entity> MatchingEntities { get; }

		internal ComponentMatcherGroup(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter = null)
			: base(componentTypes, entityFilter)
		{
			MatchingEntities = new HashSet<Entity>();
		}

		public virtual bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity))
			{
				//var cet = new ComponentEntityTuple(entity, ComponentTypes.ToDictionary(k => k, v => entity.GetComponent<>()));
				MatchingEntities.Add(entity);
				OnMatchingEntityAdded(entity);
				return true;
			}
			return false;
		}

		public void Clear()
		{
			MatchingEntities.Clear();
		}

		public override bool IsMatch(Entity entity)
		{
			return MatchingEntities.Contains(entity) || base.IsMatch(entity);
		}

		private void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntities.Remove(entity);
			entity.EntityDestroyed -= EntityOnEntityDestroyed;
			OnMatchingEntityRemoved(entity);
		}

		protected virtual void OnMatchingEntityAdded(Entity entity)
		{
			entity.EntityDestroyed += EntityOnEntityDestroyed;

			MatchingEntityAdded?.Invoke(entity);
		}

		protected virtual void OnMatchingEntityRemoved(Entity entity)
		{
			MatchingEntityRemoved?.Invoke(entity);
		}
	}

	#endregion

	#region single

	public class ComponentMatcherGroup<TComponent1> : ComponentMatcherGroup
		where TComponent1 : class, IComponent
	{
		public new event Action<ComponentEntityTuple<TComponent1>> MatchingEntityAdded;

		public new event Action<ComponentEntityTuple<TComponent1>> MatchingEntityRemoved;

		public new HashSet<ComponentEntityTuple<TComponent1>> MatchingEntities { get; }

		internal ComponentMatcherGroup(Predicate<Entity> entityFilter = null)
			: base(new[] { typeof(TComponent1) }, entityFilter)
		{
			MatchingEntities = new HashSet<ComponentEntityTuple<TComponent1>>();
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity))
			{
				var tuple = new ComponentEntityTuple<TComponent1>(entity,
					entity.GetComponent<TComponent1>());
				MatchingEntities.Add(tuple);
				OnMatchingEntityAdded(tuple);
				return true;
			}
			return false;
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1> obj)
		{
			MatchingEntityAdded?.Invoke(obj);
		}

		protected virtual void OnMatchingEntityRemoved(ComponentEntityTuple<TComponent1> obj)
		{
			MatchingEntityRemoved?.Invoke(obj);
		}
	}

	#endregion

	#region 2 tuple

	public class ComponentMatcherGroup<TComponent1, TComponent2> : ComponentMatcherGroup
		where TComponent1 : class, IComponent
		where TComponent2 : class, IComponent
	{
		public new event Action<ComponentEntityTuple<TComponent1, TComponent2>> MatchingEntityAdded;

		public new event Action<ComponentEntityTuple<TComponent1, TComponent2>> MatchingEntityRemoved;

		public new HashSet<ComponentEntityTuple<TComponent1, TComponent2>> MatchingEntities { get; }

		public ComponentMatcherGroup(Predicate<Entity> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2) }, entityFilter)
		{
			MatchingEntities = new HashSet<ComponentEntityTuple<TComponent1, TComponent2>>();
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity))
			{
				var tuple = new ComponentEntityTuple<TComponent1, TComponent2>(entity,
					entity.GetComponent<TComponent1>(),
					entity.GetComponent<TComponent2>());
				MatchingEntities.Add(tuple);
				OnMatchingEntityAdded(tuple);
				return true;
			}
			return false;
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1, TComponent2> obj)
		{
			MatchingEntityAdded?.Invoke(obj);
		}

		protected virtual void OnMatchingEntityRemoved(ComponentEntityTuple<TComponent1, TComponent2> obj)
		{
			MatchingEntityRemoved?.Invoke(obj);
		}
	}

	#endregion

	#region 3 tuple

	public class ComponentMatcherGroup<TComponent1, TComponent2, TComponent3> : ComponentMatcherGroup
		where TComponent1 : class, IComponent
		where TComponent2 : class, IComponent
		where TComponent3 : class, IComponent
	{
		public new event Action<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> MatchingEntityAdded;

		public new event Action<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> MatchingEntityRemoved;

		public new HashSet<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> MatchingEntities { get; }

		public ComponentMatcherGroup(Predicate<Entity> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3) }, entityFilter)
		{
			MatchingEntities = new HashSet<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>>();
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity))
			{
				var tuple = new ComponentEntityTuple<TComponent1, TComponent2, TComponent3>(entity,
					entity.GetComponent<TComponent1>(),
					entity.GetComponent<TComponent2>(),
					entity.GetComponent<TComponent3>());
				MatchingEntities.Add(tuple);
				OnMatchingEntityAdded(tuple);
				return true;
			}
			return false;
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1, TComponent2, TComponent3> obj)
		{
			MatchingEntityAdded?.Invoke(obj);
		}

		protected virtual void OnMatchingEntityRemoved(ComponentEntityTuple<TComponent1, TComponent2, TComponent3> obj)
		{
			MatchingEntityRemoved?.Invoke(obj);
		}
	}

	#endregion
}
