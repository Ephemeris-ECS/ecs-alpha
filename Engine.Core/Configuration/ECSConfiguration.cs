using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;

namespace Engine.Configuration
{
	// ReSharper disable once InconsistentNaming
	public class ECSConfiguration
	{
		public IEnumerable<Archetype> Archetypes { get; set; }

		public IEnumerable<SystemConfiguration> Systems { get; set; }

		public ECSConfiguration(IEnumerable<Archetype> archetypes, IEnumerable<SystemConfiguration> systems)
		{
			Archetypes = archetypes ?? new Archetype[0];
			Systems = systems ?? new SystemConfiguration[0];
		}
	}
}
