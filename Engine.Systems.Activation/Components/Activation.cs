using Engine.Components;

namespace Engine.Systems.Activation.Components
{
	public class Activation : IComponent
	{

		public ActivationState ActivationState { get; private set; } = ActivationState.NotActive;

		public void SetState(ActivationState state)
		{
			ActivationState = state;
		}
	}
}
