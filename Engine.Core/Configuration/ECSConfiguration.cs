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
		public List<Archetype> Archetypes { get; set; }

		public List<SystemConfiguration> Systems { get; set; }

		public ECSConfiguration(List<Archetype> archetypes, List<SystemConfiguration> systems)
		{
			Archetypes = archetypes ?? new List<Archetype>();
			Systems = systems ?? new List<SystemConfiguration>();
		}
	}
}
