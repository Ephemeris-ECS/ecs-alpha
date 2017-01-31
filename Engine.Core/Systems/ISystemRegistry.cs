using System.Collections.Generic;

namespace Engine.Systems
{
	// TODO: make this interface and the implementation internal as these will cause circular references and break the DI if a system depends on this
	public interface ISystemRegistry
	{
		bool TryGetSystem<TSystem>(out TSystem ssytem) where TSystem : class, ISystem;

		IList<TSystem> GetSystems<TSystem>() where TSystem : class, ISystem;

		void Initialize();
		void Tick(int currentTick);
	}
}