using Engine.Common;
using Engine.Components;

namespace Engine.Planning
{
	public class Intents : IComponent
	{
		//TODO: make this a reference to a more memory efficient storage pool
		private readonly SimpleStack<IIntent> _intents;

		public Intents() 
		{
			_intents = new SimpleStack<IIntent>();
		}

		public bool HasIntents => _intents.Any();
		
		public void Enqueue(IIntent intent)
		{
			_intents.Push(intent);
		}

		public void Replace(IIntent intent)
		{
			_intents.Clear();
			Enqueue(intent);
		}

		public IIntent Pop()
		{
			return _intents.Pop();
		}

		public bool TryPeek(out IIntent last)
		{
			return _intents.TryPeek(out last);
		}

		public bool TryPeekIntent<TIntent>(out TIntent last) 
			where TIntent : class, IIntent
		{
			IIntent intent;
			if (TryPeek(out intent))
			{
				last = intent as TIntent;
				return last != null;
			}
			last = null;
			return false;
		}
	}
}
