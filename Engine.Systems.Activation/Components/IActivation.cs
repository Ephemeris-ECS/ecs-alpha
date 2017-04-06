using Engine.Components;

namespace Engine.Systems.Activation.Components
{
	public interface IActivation : IComponent
	{
		bool CanActivate { get; }

		bool CanDeactivate { get; }

		bool IsActive { get; }

		void Activate();

		void Deactivate();

	}
}
