using System;
using Engine.Components;
//using Engine.Messaging;
using Engine.Serialization;
using Engine.Util;
using Zenject;

namespace Engine.Entities
{
	public delegate void EntityDelegate(Entity entity);

	public class Entity : ComponentContainer, IEquatable<Entity>
	{
		public int Id { get; protected set; }

		protected bool Disposed;

		public event EntityDelegate EntityDisposing;

		public event EntityDelegate EntityDisposed;

		public void Initialize(int id)
		{
			Id = id;
			Disposed = false;
			EntityDisposed = null;
			for (var i = 0; i < Components.Length; i++)
			{
				Components[i] = null;
			}
		}

		#region Event registry

		private void RaiseEntityDestroyed()
		{
			EntityDisposing?.Invoke(this);
			EntityDisposing = null;
			EntityDisposed?.Invoke(this);
			EntityDisposed = null;
		}

		public override void Dispose()
		{
			//TODO: interlock this perhaps, once we want to support multithreading
			if (Disposed == false)
			{
				Disposed = true;
				RaiseEntityDestroyed();
				base.Dispose();
			}
		}

		~Entity()
		{
			Dispose();
		}

		#endregion

		public bool Equals(Entity other)
		{
			return Id == other?.Id;
		}

		public override string ToString()
		{
			return $"Entity [{Id}] {this.GetType()}";
		}

		public virtual void OnDeserialized()
		{
		}

		public class Factory : Factory<Entity>
		{
			
		}
	}
}
