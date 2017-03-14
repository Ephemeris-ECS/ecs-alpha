using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Commands
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DeduplicateAttribute : Attribute
	{
		public DeduplicationPolicy DeduplicationPolicy { get; }

		public DeduplicateAttribute(DeduplicationPolicy deduplicationPolicy)
		{
			DeduplicationPolicy = deduplicationPolicy;
		}
	}
}
