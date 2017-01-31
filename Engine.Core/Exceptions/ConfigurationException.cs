using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Exceptions
{
	public class ConfigurationException : EngineException
	{
		public ConfigurationException()
		{
		}

		public ConfigurationException(string message)
			: base(message)
		{
		}

		public ConfigurationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
