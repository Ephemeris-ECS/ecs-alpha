using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Exceptions;
using Engine.Systems;

namespace Engine.Events
{
	public class EventSystem : ITickableSystem
	{
		private readonly Dictionary<Type, List<Subscriber>> _subscribersByType;

		private int _currentTick;

		private int _eventId;

		private class DisposableSubscription : IDisposable
		{
			private bool _disposed;

			private readonly Action<Subscriber> _dispose;

			private readonly Subscriber _subscriber;

			public DisposableSubscription(Subscriber subscriber, Action<Subscriber> dispose)
			{
				_subscriber = subscriber;
				_dispose = dispose;
			}

			public void Dispose()
			{
				if (_disposed == false)
				{
					_disposed = true;
					_dispose?.Invoke(_subscriber);
				}
			}
		}

		public EventSystem()
		{
			_subscribersByType = new Dictionary<Type, List<Subscriber>>();
		}

		public void Publish(IEvent @event)
		{
			@event.SetCounters(_currentTick, _eventId++);

			if (_subscribersByType.TryGetValue(@event.GetType(), out var typedSubscribers))
			{
				foreach (var subscriber in typedSubscribers)
				{
					subscriber.OnNext(@event);
				}
			}
		}

		//public IDisposable Subscribe(Type eventType, Action<IEvent> handler)
		//{
		//	if (typeof(IEvent).IsAssignableFrom(eventType) == false)
		//	{
		//		throw new EngineException($"Cannot subscribe to non event type {eventType}");
		//	}

		//	if (_subscribersByType.TryGetValue(eventType, out var typedSubscribers) == false)
		//	{
		//		typedSubscribers = new List<Subscriber>();
		//		_subscribersByType.Add(eventType, typedSubscribers);
		//	}
		//	var subscriber = new Subscriber<IEvent>(handler);
		//	typedSubscribers.Add(subscriber);

		//	var disposable = new DisposableSubscription(subscriber, DisposeSubscriber);
		//	return disposable;
		//}

		public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
			where TEvent : class, IEvent
		{
			var eventType = typeof(TEvent);
			if (_subscribersByType.TryGetValue(eventType, out var typedSubscribers) == false)
			{
				typedSubscribers = new List<Subscriber>();
				_subscribersByType.Add(eventType, typedSubscribers);
			}
			var subscriber = new Subscriber<TEvent>(handler);
			typedSubscribers.Add(subscriber);

			var disposable = new DisposableSubscription(subscriber, DisposeSubscriber);
			return disposable;
		}

		private void DisposeSubscriber(Subscriber subscriber)
		{
			if (_subscribersByType.TryGetValue(subscriber.HandlesType, out var typedSubscribers))
			{
				typedSubscribers.Remove(subscriber);
			}

		}

		public void Tick(int currentTick)
		{
			_currentTick = currentTick;
		}
	}
}
