using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Exceptions;

namespace Engine.Lifecycle
{
	public class LifecycleException : EngineException
	{
		public LifecycleException()
		{
		}

		public LifecycleException(string message) : base(message)
		{
		}

		public LifecycleException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
