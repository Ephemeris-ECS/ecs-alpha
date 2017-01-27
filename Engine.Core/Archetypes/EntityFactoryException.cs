using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Exceptions;

namespace Engine.Archetypes
{
	public class EntityFactoryException : EngineException
	{
		public string Archetype { get; }

		public EntityFactoryException(string archetype)
		{
			Archetype = archetype;
		}

		public EntityFactoryException(string message, string archetype)
			: base(message)
		{
			Archetype = archetype;
		}

		public EntityFactoryException(string message, Exception innerException, string archetype)
			: base(message, innerException)
		{
			Archetype = archetype;
		}
	}
}
