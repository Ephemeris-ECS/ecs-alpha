using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Events
{
	public abstract class Event : IEvent
	{
		public int Tick { get; private set; }
		public int Sequence { get; private set; }

		public void SetCounters(int tick, int sequence)
		{
			Tick = tick;
			Sequence = sequence;
		}
	}
}
