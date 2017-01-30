using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Exceptions;

namespace Engine.Archetypes
{
	internal class EntityFactoryException : EngineException
	{
		public EntityFactoryException()
		{
		}

		public EntityFactoryException(string message) : base(message)
		{
		}

		public EntityFactoryException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
