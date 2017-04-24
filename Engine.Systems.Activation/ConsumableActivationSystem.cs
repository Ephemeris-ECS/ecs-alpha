using System;
using Engine.Components;
using Engine.Systems.Activation.Components;

namespace Engine.Systems.Activation
{
	public class ConsumableActivationSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Components.Activation, ConsumableActivation> _consumableMatcherGroup;

		public ConsumableActivationSystem(IMatcherProvider matcherProvider)
		{
			_consumableMatcherGroup = matcherProvider.CreateMatcherGroup<Components.Activation, ConsumableActivation>();
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _consumableMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.Deactivating:
						OnDeactivating(match);
						break;
				}
			}
		}

		private void OnDeactivating(ComponentEntityTuple<Components.Activation, ConsumableActivation> entityTuple)
		{
			entityTuple.Component2.ActivationsRemaining--;
			if (entityTuple.Component2.ActivationsRemaining == 0)
			{
				entityTuple.Entity.Dispose();
			}
		}
	}
}
