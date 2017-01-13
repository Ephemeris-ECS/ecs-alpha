﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Components
{
	public class ComponentMatcherGroup : ComponentMatcher
	{
		public HashSet<Entity> MatchingEntities { get; }

		internal ComponentMatcherGroup(IEnumerable<Type> componentTypes)
			: base(componentTypes)
		{
			MatchingEntities = new HashSet<Entity>();
		}

		internal ComponentMatcherGroup(IEnumerable<Type> componentTypes, Predicate<Entity> entityFilter)
			: base(componentTypes, entityFilter)
		{
			MatchingEntities = new HashSet<Entity>();
		}

		public bool TryAddEntity(Entity entity)
		{
			if (IsMatch(entity))
			{
				//var cet = new ComponentEntityTuple(entity, ComponentTypes.ToDictionary(k => k, v => entity.GetComponent<>()));
				MatchingEntities.Add(entity);
				entity.EntityDestroyed += EntityOnEntityDestroyed;
				return true;
			}
			// TODO: lazy implementation - catch the entity destroyed event instead
			else
			{
				MatchingEntities.Remove(entity);
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
		}
	}
}