using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Engine.Components;

namespace Engine.Entities
{
	public class ComponentContainer : IComponentContainer
	{
		public IComponent[] Components { get; private set; }

		private bool _disposed;
		
		#region constructors

		public ComponentContainer()
		{
			Components = new IComponent[MatcherProvider.ComponentTypeMap.Count];
		}

		#endregion

		#region dispose

		~ComponentContainer()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			if (_disposed == false)
			{
				_disposed = true;
			}
		}

		#endregion

		#region components

		//TODO: there has to be  better way of storing what type of component this entity has 

		public void AddComponent(IComponent component)
		{
			int concreteComponentIndex;
			if (MatcherProvider.ComponentTypeMap.TryGetValue(component.GetType(), out concreteComponentIndex) == false)
			{
				Debugger.Break();
			}

			Components[concreteComponentIndex] = component;
		}

		public bool HasComponent<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponentsInternal<TComponentInterface>().Any();
		}

		public TComponentInterface GetComponent<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponentsInternal<TComponentInterface>().Single();
		}

		//public IComponent GetComponent(Type componentType)
		//{
		//	return GetComponentsInternal<com>().Single();
		//}

		public bool TryGetComponent<TComponentInterface>(out TComponentInterface component)
			where TComponentInterface : class, IComponent
		{
			component = GetComponentsInternal<TComponentInterface>().SingleOrDefault();
			return component != null;
		}

		public bool TryGetComponent(Type componentType, out IComponent component)
		{
			component = GetComponentsInternal(componentType).SingleOrDefault();
			return component != null;
		}

		public IEnumerable<TComponentInterface> GetComponents<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponentsInternal<TComponentInterface>();
		}

		//private IEnumerable<IComponent> GetComponentsInternal(Type componentType)
		//{
		//	//TODO: this looks like a m * n problem
		//	HashSet<Type> componentTypes;
		//	if (MatcherProvider.ComponentTypesByImplementation.TryGetValue(componentType, out componentTypes))
		//	{
		//		return componentTypes.Where(ct => Components.ContainsKey(ct))
		//			.Select(ct => Components[ct]);
		//	}
		//	return new IComponent[0];
		//}

		private bool TryGetConcreteComponent<TComponentInterface>(out TComponentInterface component)
			where TComponentInterface : class, IComponent
		{
			IComponent componentInternal;
			var success = TryGetConcreteComponent(typeof(TComponentInterface), out componentInternal);
			component = componentInternal as TComponentInterface;
			return success;
		}

		private bool TryGetConcreteComponent(Type componentType, out IComponent component)
		{
			int concreteIndex;
			if (MatcherProvider.ComponentTypeMap.TryGetValue(componentType, out concreteIndex))
			{
				return TryGetConcreteComponent(concreteIndex, out component);
			}
			component = null;
			return false;
		}

		private bool TryGetConcreteComponent(int componentIndex, out IComponent component)
		{
			if (Components[componentIndex] == null)
			{
				component = null;
				return false;
			}
			component = Components[componentIndex];
			return true;
		}

		private IEnumerable<TComponentInterface> GetComponentsInternal<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			var components = new List<TComponentInterface>();

			if (TryGetConcreteComponent<TComponentInterface>(out var component))
			{
				components.Add(component);
			}

			//TODO: this looks like a m * n problem
			HashSet<int> componentTypeIds;
			if (MatcherProvider.ComponentTypesByImplementation.TryGetValue(typeof(TComponentInterface), out componentTypeIds))
			{
				foreach (var concreteComponentTypeId in componentTypeIds)
				{
					IComponent concreteComponent;
					if (TryGetConcreteComponent(concreteComponentTypeId, out concreteComponent))
					{
						components.Add(concreteComponent as TComponentInterface);
					}
				}
			}
			return components;
		}

		private IEnumerable<IComponent> GetComponentsInternal(Type componentType)
		{
			var components = new List<IComponent>();

			if (TryGetConcreteComponent(componentType, out var component))
			{
				components.Add(component);
			}

			//TODO: this looks like a m * n problem
			HashSet<int> componentTypeIds;
			if (MatcherProvider.ComponentTypesByImplementation.TryGetValue(componentType, out componentTypeIds))
			{
				foreach (var concreteComponentTypeId in componentTypeIds)
				{
					IComponent concreteComponent;
					if (TryGetConcreteComponent(concreteComponentTypeId, out concreteComponent))
					{
						components.Add(concreteComponent);
					}
				}
			}
			return components;
		}

		public bool HasComponentsImplmenting<TComponentInterface>()
			where TComponentInterface : class, IComponent
		{
			return GetComponents<TComponentInterface>().Any();
		}

		#endregion
	}

}

