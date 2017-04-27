using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;

namespace Engine.Lifecycle.Events
{
	public class EndGameEvent : Event
	{
		public EndGameState State { get; set; }
	}
}
