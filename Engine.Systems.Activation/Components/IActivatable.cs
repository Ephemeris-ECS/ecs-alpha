using Engine.Components;
using Engine.Entities;

namespace Engine.Systems.Activation.Components
{
	public interface IActivatable : IComponent
	{
		bool CanActivate(Entity activator, Entity target);

		void OnActivating(Entity target);

		void OnActive(Entity target);

		void OnDeactivating(Entity target);

	}
}
