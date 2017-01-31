using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Exceptions
{
	public class SystemException : EngineException
	{
		public SystemException()
		{
		}

		public SystemException(string message)
			: base(message)
		{
		}

		public SystemException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
