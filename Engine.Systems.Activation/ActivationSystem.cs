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
		private readonly List<IActivationExtension> _activationExtensions;
		
		private readonly ComponentMatcherGroup<Components.Activation> _activationMatcher;

		public ActivationSystem(IMatcherProvider matcherProvider, 
			// TODO: remove zenject dependency when implicit optional collection paramters is implemented
			[InjectOptional] List<IActivationExtension> activationExtensions) 
		{
			_activationExtensions = activationExtensions;
			
			_activationMatcher = matcherProvider.CreateMatcherGroup<Components.Activation>();
		}

		public void Tick(int currentTick)
		{
			foreach (var match in _activationMatcher.MatchingEntities)
			{
				var activation = match.Component1;
				switch (activation.ActivationState)
				{
					case ActivationState.NotActive:
						ExecuteActivationExtensionActions(iax => iax.OnNotActive(match.Entity.Id, activation));
						continue;
					case ActivationState.Activating:
						activation.SetState(ActivationState.Active);
						ExecuteActivationExtensionActions(iax => iax.OnActivating(match.Entity.Id, activation));
						break;
					case ActivationState.Deactivating:
						activation.SetState(ActivationState.NotActive);
						ExecuteActivationExtensionActions(iax => iax.OnDeactivating(match.Entity.Id, activation));
						break;
					case ActivationState.Active:
						ExecuteActivationExtensionActions(iax => iax.OnActive(match.Entity.Id, activation));
						break;
				}
			}
		}

		private void ExecuteActivatableAction(Entity entity, Action<IActivatable> action)
		{
			foreach (var activatable in entity.GetComponents<IActivatable>())
			{
				action(activatable);
			}
		}
		
		private void ExecuteActivationExtensionActions(Action<IActivationExtension> action)
		{
			foreach (var activationExtension in _activationExtensions)
			{
				action(activationExtension);
			}
		}

	}
}
