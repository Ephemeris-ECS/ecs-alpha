using System;
using Engine.Components;
using Engine.Systems.Activation.Components;

namespace Engine.Systems.Activation
{
	public class ConsumableActivationExtension : IActivationExtension
	{
		private readonly ComponentMatcherGroup<Components.Activation, ConsumableActivation> _consumableMatcherGroup;

		public ConsumableActivationExtension(IMatcherProvider matcherProvider)
		{
			_consumableMatcherGroup = matcherProvider.CreateMatcherGroup<Components.Activation, ConsumableActivation>();
		}

		public void OnNotActive(int itemId, Components.Activation activation)
		{
		}

		public void OnActivating(int itemId, Components.Activation activation)
		{
		}

		public void OnActive(int itemId, Components.Activation activation)
		{
		}

		public void OnDeactivating(int itemId, Components.Activation activation)
		{
			if (_consumableMatcherGroup.TryGetMatchingEntity(itemId, out var itemTuple))
			{
				itemTuple.Component2.ActivationsRemaining--;
				if (itemTuple.Component2.ActivationsRemaining == 0)
				{
					itemTuple.Entity.Dispose();
				}
			}
		}
	}
}
