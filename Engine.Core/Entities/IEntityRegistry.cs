using System;
using System.Collections.Generic;

namespace Engine.Entities
{
	public interface IEntityRegistry
	{
		EntityDictionary Entities { get; }
		int NextEntityId { get; }

		Entity CreateEntity();
		bool TryGetEntityById(int id, out Entity entity);

		event Action<int> EntityDestroyed;
	}
}