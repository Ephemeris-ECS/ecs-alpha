using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Archetypes
{
	public abstract class ArchetypeFactory
	{
		protected static Func<Archetype> Factory { get; set; }

	}
}
