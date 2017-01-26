using System;
using System.Collections.Generic;
using System.Threading;
using Engine.Components;
using Engine.Serialization;

namespace Engine.Entities
{
	public class EntityRegistry : IEntityRegistry
	{
		private int _entitySeed;

		private Entity.Factory _entityFactory;

		public EntityDictionary Entities { get; }

		public Queue<Entity> EntityPool { get; }

		public int NextEntityId => Interlocked.Increment(ref _entitySeed);

		private readonly object _entityPoolLock = new object();
		private readonly object _entityLock = new object();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="entityDictionary">this is is injected so that the DI system can ensure it is a singleton and can be resused by the serializer</param>
		/// <param name="entityFactory"></param>
		public EntityRegistry(EntityDictionary entityDictionary, Entity.Factory entityFactory)
		{
			_entityFactory = entityFactory;
			Entities = entityDictionary;
			EntityPool = new Queue<Entity>();
		}

		public Entity CreateEntity()
		{
			lock (_entityPoolLock)
			{
				var entity = EntityPool.Count > 0 ? EntityPool.Dequeue() : _entityFactory.Create();
				entity.Initialize(NextEntityId);
				entity.EntityDestroyed += EntityOnEntityDestroyed;
				AddEntity(entity);
				return entity;
			}
		}

		private void EntityOnEntityDestroyed(Entity entity)
		{
			lock (_entityPoolLock)
			{
				Entities.Remove(entity.Id);
				EntityPool.Enqueue(entity);
			}
		}

		private void AddEntity(Entity entity)
		{
			lock (_entityPoolLock)
			{
				if (Entities.ContainsKey(entity.Id))
				{
					throw new EntityRegistryException($"A duplicate entity was created with id {entity.Id}");
				}
				Entities.Add(entity.Id, entity);
			}
		}

		public bool TryGetEntityById(int id, out Entity entity)
		{
			Entity gameEntity;
			var found = Entities.TryGetValue(id, out gameEntity);
			entity = gameEntity;
			return found;
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}
	}
}
