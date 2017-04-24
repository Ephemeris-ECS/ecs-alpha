using Engine.Components;

namespace Engine.Systems.Activation.Components
{
	[ComponentDependency(typeof(Activation))]
	public class TimedActivation : IComponent
	{
		public decimal ActivationTickModifier { get; set; } = 1m;

		public decimal ActivationTicksRemaining { get; set; }

		public int ActivationDuration { get; set; }

		public bool Synchronized { get; set; }
	}
}
