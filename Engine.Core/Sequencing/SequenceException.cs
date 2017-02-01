using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Exceptions;

namespace Engine.Sequencing
{
	public class SequenceException : EngineException
	{
		public SequenceException()
		{
		}

		public SequenceException(string message)
			: base(message)
		{
		}

		public SequenceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
