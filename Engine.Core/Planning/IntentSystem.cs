﻿using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;
using Engine.Systems;
using Zenject;

namespace Engine.Planning
{
	public class IntentSystem : ITickableSystem
	{
		private readonly ComponentMatcherGroup<Intents> _intentsMatcher;

		private readonly Dictionary<Type, IIntentProcessor> _intentHandlers;

		public IntentSystem(IMatcherProvider matcherProvider, 
			[InjectOptional] List<IIntentProcessor> intentHandlers)
		{
			_intentsMatcher = matcherProvider.CreateMatcherGroup<Intents>();
			_intentHandlers = intentHandlers.ToDictionary(k => k.HandlesIntent, v => v);
		}

		public bool TryProcessIntent(IIntent intent)
		{
			IIntentProcessor intentProcessor;
			if (_intentHandlers.TryGetValue(intent.GetType(), out intentProcessor))
			{
				return intentProcessor.TryProcessIntent(intent);
			}
			return false;
		}

		public void Tick(int currentTick)
		{
			foreach (var agent in _intentsMatcher.MatchingEntities)
			{
				IIntent intent;
				if (agent.Component1.TryPeek(out intent))
				{
					if (TryProcessIntent(intent))
					{
						agent.Component1.Pop();
					}
				}
			}
		}

		public void Dispose()
		{
			// nothing to dispose
		}
	}
}
