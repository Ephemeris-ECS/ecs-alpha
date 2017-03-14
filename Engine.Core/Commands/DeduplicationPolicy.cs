using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Commands
{
	public enum DeduplicationPolicy
	{
		Undefined = 0,

		/// <summary>
		/// Replace duplicate values present in the queue (LIFO)
		/// </summary>
		Replace,

		/// <summary>
		/// Discard if there are duplicate values present in the queue (FIFO)
		/// </summary>
		Discard,

		/// <summary>
		/// Allow duplicate values in the queue
		/// </summary>
		Allow,
	}
}
