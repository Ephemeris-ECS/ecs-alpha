using Engine.Components;
using Engine.Systems.Activation.Components;

namespace Engine.Systems.Activation
{
	public class TimedActivationsystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Components.Activation, TimedActivation> _timedActivationMatcherGroup;

		public TimedActivationsystem(IMatcherProvider matcherProvider)
		{
			_timedActivationMatcherGroup = matcherProvider.CreateMatcherGroup<Components.Activation, TimedActivation>();
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _timedActivationMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.Activating:
						OnActivating(match, currentTick);
						break;
					case ActivationState.Active:
						OnActive(match, currentTick);
						break;
				}
			}
		}

		private void OnActivating(ComponentEntityTuple<Components.Activation, TimedActivation> entityTuple, int currentTick)
		{
			entityTuple.Component2.ActivationTicksRemaining = entityTuple.Component2.ActivationDuration;
		}

		private void OnActive(ComponentEntityTuple<Components.Activation, TimedActivation> entityTuple, int currentTick)
		{
			if (entityTuple.Component2.Synchronized == false)
			{
				entityTuple.Component2.ActivationTicksRemaining -= entityTuple.Component2.ActivationTickModifier;
				if (entityTuple.Component2.ActivationTicksRemaining <= 0)
				{
					entityTuple.Component1.SetState(ActivationState.Deactivating, currentTick);
					entityTuple.Component2.ActivationTicksRemaining = 0;
				}
			}
		}

		public void Dispose()
		{
			_timedActivationMatcherGroup?.Dispose();
		}
	}
}
