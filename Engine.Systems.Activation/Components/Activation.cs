using Engine.Components;

namespace Engine.Systems.Activation.Components
{
	public class Activation : IComponent
	{
		public ActivationState ActivationState { get; private set; } = ActivationState.NotActive;

		public void Activate()
		{
			ActivationState = ActivationState.Activating;
		}

		public void SetState(ActivationState state)
		{
			ActivationState = state;
		}

		public void Deactivate()
		{
			ActivationState = ActivationState.Deactivating;
		}

		public void CancelActivation()
		{
			ActivationState = ActivationState.NotActive;
		}
	}
}
