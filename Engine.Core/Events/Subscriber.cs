using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Events
{
	public abstract class Subscriber
	{
		public abstract Type HandlesType { get; }

		public abstract void OnNext(IEvent @event);
	}

	public class Subscriber<TEvent> : Subscriber
		where TEvent : class, IEvent
	{
		private readonly Action<TEvent> _handler;

		public override Type HandlesType => typeof(TEvent);

		public Subscriber(Action<TEvent> handler)
		{
			_handler = handler;
		}

		public override void OnNext(IEvent @event)
		{
			Next(@event as TEvent);
		}

		private void Next(TEvent @event)
		{
			if (@event != null)
			{
				_handler?.Invoke(@event);
			}
		}
	}
}
