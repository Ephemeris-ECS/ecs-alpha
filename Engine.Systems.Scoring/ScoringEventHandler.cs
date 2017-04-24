using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Events;

namespace Engine.Systems.Scoring
{
	public abstract class ScoringEventHandler<TEvent> : Events.EventHandler<TEvent>, IScoringExtension
		where TEvent : class, IEvent
	{
		protected ScoringEventHandler(EventSystem eventSystem) 
			: base(eventSystem)
		{
		}
	}
}
