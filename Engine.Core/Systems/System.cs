using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Entities;

namespace Engine.Systems
{
	public class System : ISystem
	{
		protected IMatcherProvider MatcherProvider { get; }

		protected IEntityRegistry EntityRegistry { get; }


		public System(IMatcherProvider matcherProvider, 
			IEntityRegistry entityRegistry)
		{
			MatcherProvider = matcherProvider;
			EntityRegistry = entityRegistry;
		}
	}
}
