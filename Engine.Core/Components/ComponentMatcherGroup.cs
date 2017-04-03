using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	#region pure match, no result tuple

	public abstract class ComponentMatcherGroup : ComponentMatcher
	{
		public event Action<Entity> MatchingEntityAdded;

		public event Action<Entity> MatchingEntityRemoved;


		protected readonly Dictionary<int, Entity> MatchingEntityDictionary;

		public Entity[] MatchingEntities => MatchingEntityDictionary.Values.ToArray();

		internal ComponentMatcherGroup(Type[] componentTypes)
			: base(componentTypes)
		{
			MatchingEntityDictionary = new Dictionary<int, Entity>();
		}

		public bool TryGetMatchingEntity(int id, out Entity entity)
		{
			return MatchingEntityDictionary.TryGetValue(id, out entity);
		}

		public virtual bool TryAddEntity(Entity entity)
		{
			if (IsTypeMatch(entity))
			{
				if (MatchingEntityDictionary.ContainsKey(entity.Id) == false)
				{
					MatchingEntityDictionary.Add(entity.Id, entity);
					OnMatchingEntityAdded(entity);
				}
				return true;
			}
			return false;
		}

		public void Clear()
		{
			MatchingEntityDictionary.Clear();
		}

		private void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntityDictionary.Remove(entity.Id);
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

		private Predicate<ComponentEntityTuple<TComponent1>> EntityFilter { get; }
		
		protected new readonly Dictionary<int, ComponentEntityTuple<TComponent1>> MatchingEntityDictionary;

		public new ComponentEntityTuple<TComponent1>[] MatchingEntities => MatchingEntityDictionary.Values.ToArray();

		public int[] MatchingEntityKeys => MatchingEntityDictionary.Keys.ToArray();

		internal ComponentMatcherGroup(Predicate<ComponentEntityTuple<TComponent1>> entityFilter = null)
			: base(new[] { typeof(TComponent1) })
		{
			EntityFilter = entityFilter;
			MatchingEntityDictionary = new Dictionary<int, ComponentEntityTuple<TComponent1>>();
		}

		public bool TryGetMatchingEntity(int id, out ComponentEntityTuple<TComponent1> tuple)
		{
			return MatchingEntityDictionary.TryGetValue(id, out tuple);
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsTypeMatch(entity))
			{
				if (MatchingEntityDictionary.ContainsKey(entity.Id) == false)
				{
					var tuple = new ComponentEntityTuple<TComponent1>(entity, entity.GetComponent<TComponent1>());
					if (EntityFilter == null || EntityFilter(tuple))
					{
						MatchingEntityDictionary.Add(entity.Id, tuple);
						OnMatchingEntityAdded(tuple);
						return true;
					}
				}
			}
			return false;
		}

		protected void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntityDictionary.Remove(entity.Id);
			entity.EntityDestroyed -= EntityOnEntityDestroyed;
			OnMatchingEntityRemoved(entity);
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1> tuple)
		{
			tuple.Entity.EntityDestroyed += EntityOnEntityDestroyed;
			MatchingEntityAdded?.Invoke(tuple);
		}
	}

	#endregion

	#region 2 tuple

	public class ComponentMatcherGroup<TComponent1, TComponent2> : ComponentMatcherGroup
		where TComponent1 : class, IComponent
		where TComponent2 : class, IComponent
	{
		public new event Action<ComponentEntityTuple<TComponent1, TComponent2>> MatchingEntityAdded;

		private Predicate<ComponentEntityTuple<TComponent1, TComponent2>> EntityFilter { get; }

		/// <summary>
		/// Dictionary of matching entity tuples keyed by entity id.
		/// This should always be projected to a array or similar hwen enumerating as disposing entities will modify the collection
		/// </summary>
		protected new readonly Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2>> MatchingEntityDictionary;

		// TODO: test if key/value collection to array is expensive when done on demand, this could be cached and only reevaluated when an entity is added or removed
		public new ComponentEntityTuple<TComponent1, TComponent2>[] MatchingEntities => MatchingEntityDictionary.Values.ToArray();
		public int[] MatchingEntityKeys => MatchingEntityDictionary.Keys.ToArray();

		internal ComponentMatcherGroup(Predicate<ComponentEntityTuple<TComponent1, TComponent2>> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2) })
		{
			EntityFilter = entityFilter;
			MatchingEntityDictionary = new Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2>>();
		}

		public bool TryGetMatchingEntity(int id, out ComponentEntityTuple<TComponent1, TComponent2> tuple)
		{
			return MatchingEntityDictionary.TryGetValue(id, out tuple);
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsTypeMatch(entity))
			{
				if (MatchingEntityDictionary.ContainsKey(entity.Id) == false)
				{
					var tuple = new ComponentEntityTuple<TComponent1, TComponent2>(entity,
						entity.GetComponent<TComponent1>(),
						entity.GetComponent<TComponent2>());
					if (EntityFilter == null || EntityFilter(tuple))
					{
						MatchingEntityDictionary.Add(entity.Id, tuple);
						OnMatchingEntityAdded(tuple);
						return true;
					}
				}
			}
			return false;
		}
		protected void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntityDictionary.Remove(entity.Id);
			entity.EntityDestroyed -= EntityOnEntityDestroyed;
			OnMatchingEntityRemoved(entity);
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1, TComponent2> tuple)
		{
			tuple.Entity.EntityDestroyed += EntityOnEntityDestroyed;
			MatchingEntityAdded?.Invoke(tuple);
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

		private Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> EntityFilter { get; }
		
		/// <summary>
		/// Dictionary of matching entity tuples keyed by entity id.
		/// This should always be projected to a array or similar hwen enumerating as disposing entities will modify the collection
		/// </summary>
		protected new readonly Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> MatchingEntityDictionary;

		public new ComponentEntityTuple<TComponent1, TComponent2, TComponent3>[] MatchingEntities => MatchingEntityDictionary.Values.ToArray();
		public int[] MatchingEntityKeys => MatchingEntityDictionary.Keys.ToArray();

		internal ComponentMatcherGroup(Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3) })
		{
			EntityFilter = entityFilter;
			MatchingEntityDictionary = new Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3>>();
		}

		public bool TryGetMatchingEntity(int id, out ComponentEntityTuple<TComponent1, TComponent2, TComponent3> tuple)
		{
			return MatchingEntityDictionary.TryGetValue(id, out tuple);
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsTypeMatch(entity))
			{
				if(MatchingEntityDictionary.ContainsKey(entity.Id) == false)
				{
					var tuple = new ComponentEntityTuple<TComponent1, TComponent2, TComponent3>(entity,
						entity.GetComponent<TComponent1>(),
						entity.GetComponent<TComponent2>(),
						entity.GetComponent<TComponent3>());
					if (EntityFilter == null || EntityFilter(tuple))
					{
						MatchingEntityDictionary.Add(entity.Id, tuple);
						OnMatchingEntityAdded(tuple);
						return true;
					}
				}
			}
			return false;
		}

		protected void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntityDictionary.Remove(entity.Id);
			entity.EntityDestroyed -= EntityOnEntityDestroyed;
			OnMatchingEntityRemoved(entity);
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1, TComponent2, TComponent3> tuple)
		{
			tuple.Entity.EntityDestroyed += EntityOnEntityDestroyed;
			MatchingEntityAdded?.Invoke(tuple);
		}

	}

	#endregion

	#region 4 tuple

	public class ComponentMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4> : ComponentMatcherGroup
		where TComponent1 : class, IComponent
		where TComponent2 : class, IComponent
		where TComponent3 : class, IComponent
		where TComponent4 : class, IComponent
	{
		public new event Action<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>> MatchingEntityAdded;

		private Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>> EntityFilter { get; }

		/// <summary>
		/// Dictionary of matching entity tuples keyed by entity id.
		/// This should always be projected to a array or similar hwen enumerating as disposing entities will modify the collection
		/// </summary>
		protected new readonly Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>> MatchingEntityDictionary;

		public new ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>[] MatchingEntities => MatchingEntityDictionary.Values.ToArray();
		public int[] MatchingEntityKeys => MatchingEntityDictionary.Keys.ToArray();
		
		internal ComponentMatcherGroup(Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3), typeof(TComponent4) })
		{
			EntityFilter = entityFilter;
			MatchingEntityDictionary = new Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>>();
		}

		public bool TryGetMatchingEntity(int id, out ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4> tuple)
		{
			return MatchingEntityDictionary.TryGetValue(id, out tuple);
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsTypeMatch(entity))
			{
				if (MatchingEntityDictionary.ContainsKey(entity.Id) == false)
				{
					var tuple = new ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4>(entity,
						entity.GetComponent<TComponent1>(),
						entity.GetComponent<TComponent2>(),
						entity.GetComponent<TComponent3>(),
						entity.GetComponent<TComponent4>());
					if (EntityFilter == null || EntityFilter(tuple))
					{
						MatchingEntityDictionary.Add(entity.Id, tuple);
						OnMatchingEntityAdded(tuple);
						return true;
					}
				}
			}
			return false;
		}

		protected void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntityDictionary.Remove(entity.Id);
			entity.EntityDestroyed -= EntityOnEntityDestroyed;
			OnMatchingEntityRemoved(entity);
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4> tuple)
		{
			tuple.Entity.EntityDestroyed += EntityOnEntityDestroyed;
			MatchingEntityAdded?.Invoke(tuple);
		}

	}

	#endregion

	#region 5 tuple

	public class ComponentMatcherGroup<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> : ComponentMatcherGroup
		where TComponent1 : class, IComponent
		where TComponent2 : class, IComponent
		where TComponent3 : class, IComponent
		where TComponent4 : class, IComponent
		where TComponent5 : class, IComponent
	{
		public new event Action<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>> MatchingEntityAdded;

		private Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>> EntityFilter { get; }

		/// <summary>
		/// Dictionary of matching entity tuples keyed by entity id.
		/// This should always be projected to a array or similar hwen enumerating as disposing entities will modify the collection
		/// </summary>
		protected new readonly Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>> MatchingEntityDictionary;

		public new ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>[] MatchingEntities => MatchingEntityDictionary.Values.ToArray();
		public int[] MatchingEntityKeys => MatchingEntityDictionary.Keys.ToArray();

		internal ComponentMatcherGroup(Predicate<ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3), typeof(TComponent4), typeof(TComponent5) })
		{
			EntityFilter = entityFilter;
			MatchingEntityDictionary = new Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>>();
		}

		public bool TryGetMatchingEntity(int id, out ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> tuple)
		{
			return MatchingEntityDictionary.TryGetValue(id, out tuple);
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsTypeMatch(entity))
			{
				if (MatchingEntityDictionary.ContainsKey(entity.Id) == false)
				{
					var tuple = new ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5>(entity,
						entity.GetComponent<TComponent1>(),
						entity.GetComponent<TComponent2>(),
						entity.GetComponent<TComponent3>(),
						entity.GetComponent<TComponent4>(),
						entity.GetComponent<TComponent5>());
					if (EntityFilter == null || EntityFilter(tuple))
					{
						MatchingEntityDictionary.Add(entity.Id, tuple);
						OnMatchingEntityAdded(tuple);
						return true;
					}
				}
			}
			return false;
		}

		protected void EntityOnEntityDestroyed(Entity entity)
		{
			MatchingEntityDictionary.Remove(entity.Id);
			entity.EntityDestroyed -= EntityOnEntityDestroyed;
			OnMatchingEntityRemoved(entity);
		}

		protected virtual void OnMatchingEntityAdded(ComponentEntityTuple<TComponent1, TComponent2, TComponent3, TComponent4, TComponent5> tuple)
		{
			tuple.Entity.EntityDestroyed += EntityOnEntityDestroyed;
			MatchingEntityAdded?.Invoke(tuple);
		}

	}

	#endregion
}
