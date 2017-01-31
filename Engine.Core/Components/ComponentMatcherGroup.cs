﻿using System;
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

		private readonly HashSet<Entity> _matchingEntities;

		public IEnumerable<Entity> MatchingEntities => _matchingEntities;

		internal ComponentMatcherGroup(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter = null)
			: base(componentTypes, entityFilter)
		{
			_matchingEntities = new HashSet<Entity>();
		}

		public virtual bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity))
			{
				//var cet = new ComponentEntityTuple(entity, ComponentTypes.ToDictionary(k => k, v => entity.GetComponent<>()));
				_matchingEntities.Add(entity);
				OnMatchingEntityAdded(entity);
				return true;
			}
			return false;
		}

		public void Clear()
		{
			_matchingEntities.Clear();
		}

		public override bool IsMatch(Entity entity)
		{
			return MatchingEntities.Contains(entity) || base.IsMatch(entity);
		}

		private void EntityOnEntityDestroyed(Entity entity)
		{
			_matchingEntities.Remove(entity);
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

		private readonly Dictionary<int, ComponentEntityTuple<TComponent1>> _matchingEntities;

		public new IEnumerable<ComponentEntityTuple<TComponent1>> MatchingEntities => _matchingEntities.Values;

		internal ComponentMatcherGroup(Predicate<Entity> entityFilter = null)
			: base(new[] { typeof(TComponent1) }, entityFilter)
		{
			_matchingEntities = new Dictionary<int, ComponentEntityTuple<TComponent1>>();
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity) && _matchingEntities.ContainsKey(entity.Id) == false)
			{
				var tuple = new ComponentEntityTuple<TComponent1>(entity,
					entity.GetComponent<TComponent1>());
				_matchingEntities.Add(entity.Id, tuple);
				OnMatchingEntityAdded(tuple);
				return true;
			}
			return false;
		}

		protected void EntityOnEntityDestroyed(Entity entity)
		{
			_matchingEntities.Remove(entity.Id);
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

		private readonly Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2>> _matchingEntities;

		public new IEnumerable<ComponentEntityTuple<TComponent1, TComponent2>> MatchingEntities => _matchingEntities.Values;

		public ComponentMatcherGroup(Predicate<Entity> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2) }, entityFilter)
		{
			_matchingEntities = new Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2>>();
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity) && _matchingEntities.ContainsKey(entity.Id) == false)
			{
				var tuple = new ComponentEntityTuple<TComponent1, TComponent2>(entity,
					entity.GetComponent<TComponent1>(),
					entity.GetComponent<TComponent2>());
				_matchingEntities.Add(entity.Id, tuple);
				OnMatchingEntityAdded(tuple);
				return true;
			}
			return false;
		}
		protected void EntityOnEntityDestroyed(Entity entity)
		{
			_matchingEntities.Remove(entity.Id);
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

		private readonly Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> _matchingEntities;

		public new IEnumerable<ComponentEntityTuple<TComponent1, TComponent2, TComponent3>> MatchingEntities => _matchingEntities.Values;

		public ComponentMatcherGroup(Predicate<Entity> entityFilter = null)
			: base(new[] { typeof(TComponent1), typeof(TComponent2), typeof(TComponent3) }, entityFilter)
		{
			_matchingEntities = new Dictionary<int, ComponentEntityTuple<TComponent1, TComponent2, TComponent3>>();
		}

		public override bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity) && _matchingEntities.ContainsKey(entity.Id) == false)
			{
				var tuple = new ComponentEntityTuple<TComponent1, TComponent2, TComponent3>(entity,
					entity.GetComponent<TComponent1>(),
					entity.GetComponent<TComponent2>(),
					entity.GetComponent<TComponent3>());
				_matchingEntities.Add(entity.Id, tuple);
				OnMatchingEntityAdded(tuple);
				return true;
			}
			return false;
		}

		protected void EntityOnEntityDestroyed(Entity entity)
		{
			_matchingEntities.Remove(entity.Id);
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
}
