using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Engine.Exceptions;
using Zenject;

namespace Engine.Systems
{
	public sealed class SystemRegistry : ISystemRegistry
	{
		private readonly List<ISystem> _systems;

		private readonly DiContainer _container;

		public SystemRegistry(DiContainer container, 
			[InjectOptional] List<ISystem> systems)
		{
			_systems = systems ?? new List<ISystem>();
			_container = container;
		}

		/// <summary>
		/// No TryGet here, you better know that a system exists when you request it!
		/// </summary>
		/// <typeparam name="TSystem"></typeparam>
		/// <returns></returns>
		public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
		{
			try
			{
				var system = _container.Resolve<TSystem>();
				return system;
			}
			catch (ZenjectException zex)
			{
				throw new InvalidOperationException($"System of type {typeof(TSystem)} not registered.");
			}
		}

		public bool TryGetSystem<TSystem>(out TSystem system) where TSystem : class, ISystem
		{
			try
			{
				system = _container.Resolve<TSystem>();
				return true;
			}
			catch (ZenjectException zex)
			{
				system = null;
				return false;
			}
		}

		public IList<TSystem> GetSystems<TSystem>() where TSystem : class, ISystem
		{
			try
			{
				var systems = _container.Resolve<List<TSystem>>();
				return systems;
			}
			catch (ZenjectException zex)
			{
				return new TSystem[0];
			}
		}

		public void Initialize()
		{
			foreach (var system in _systems.OfType<IInitializingSystem>())
			{
				try
				{
					system.Initialize();
				}
				catch (Exception ex)
				{
					throw new Engine.Exceptions.SystemException($"Error initializing system '{system.GetType()}'", ex);
				}
			}
		}

		// TODO: implement system to system message bus
		public void Tick(int currentTick)
		{
			foreach (var system in _systems.OfType<ITickableSystem>())
			{
				system.Tick(currentTick);
			}
		}
	}


}
