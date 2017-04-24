using System;

namespace Engine.Events
{
	public abstract class EventHandler<TEvent> : IDisposable
		where TEvent : class, IEvent
	{
		public Type HandlesType => typeof(TEvent);

		private readonly IDisposable _eventSubscription;

		protected EventHandler(EventSystem eventSystem)
		{
			_eventSubscription = eventSystem.Subscribe<TEvent>(HandleEvent);
		}

		//public void HandleEvent(IEvent @event)
		//{
		//	var typedEvent = @event as TEvent;
		//	HandleEvent(typedEvent);
		//}

		protected abstract void HandleEvent(TEvent @event);

		public void Dispose()
		{
			_eventSubscription.Dispose();
		}
	}
}
