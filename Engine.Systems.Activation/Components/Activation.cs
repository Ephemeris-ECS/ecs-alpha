using Engine.Components;

namespace Engine.Systems.Activation.Components
{
	public class Activation : IComponent
	{
		public int LastTick { get; private set; }

		public ActivationState ActivationState { get; private set; } = ActivationState.NotActive;

		public void SetState(ActivationState state, int currentTick)
		{
			if (currentTick > LastTick)
			{
				ActivationState = state;
				LastTick = currentTick;
			}
		}
	}
}
