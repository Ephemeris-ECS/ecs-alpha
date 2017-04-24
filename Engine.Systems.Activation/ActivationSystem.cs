using System;
using System.Collections.Generic;
using Engine.Components;
using Engine.Entities;
using Engine.Systems.Activation.Components;
using Zenject;

namespace Engine.Systems.Activation
{
	public class ActivationSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Components.Activation> _activationMatcherGroup;

		public ActivationSystem(IMatcherProvider matcherProvider)
		{
			_activationMatcherGroup = matcherProvider.CreateMatcherGroup<Components.Activation>();
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _activationMatcherGroup.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
					case ActivationState.Active:
						continue;

					case ActivationState.Activating:
						activation.SetState(ActivationState.Active);
						break;

					case ActivationState.Deactivating:
						activation.SetState(ActivationState.NotActive);
						break;
				}
			}
		}
	}
}
