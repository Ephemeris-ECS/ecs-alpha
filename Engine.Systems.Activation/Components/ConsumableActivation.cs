using Engine.Components;

namespace Engine.Systems.Activation.Components
{
	public class ConsumableActivation : IComponent
	{
		public int ActivationsRemaining { get; set; }

		public int TotalActivations { get; set; }
	}
}
