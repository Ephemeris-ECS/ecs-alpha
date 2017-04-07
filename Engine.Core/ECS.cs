using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Commands;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Lifecycle;
using Engine.Systems;
using Zenject;

//using System.Reactive.Disposables;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECS<TConfiguration> : IDisposable, IECS<TConfiguration>
		where TConfiguration : ECSConfiguration
	{
		public event Action<IEnumerable<ICommand>, int> CommandsProcessed;

		private bool _disposed;

		private int _currentTick;

		public int CurrentTick => _currentTick;


		public TConfiguration Configuration { get; }

		/// <summary>
		/// This is where the entity pool lives and new entities are created
		/// </summary>
		public IEntityRegistry EntityRegistry { get; private set; }

		public EntityDictionary Entities => EntityRegistry.Entities;


		/// <summary>
		/// This is where the component pool lives and component to entity mappings take place
		/// </summary>
		public IMatcherProvider MatcherProvider { get; private set; }

		/// <summary>
		/// This is where the system pool lives and systems are activated 
		/// </summary>
		protected ISystemRegistry SystemRegistry { get; private set; }

		protected IEntityFactoryProvider EntityFactoryProvider { get; }

		protected CommandQueue CommandQueue { get; }

		protected ECS(TConfiguration configuration,
			IEntityRegistry entityRegistry,
			IMatcherProvider matcherProvider,
			ISystemRegistry systemRegistry,
			IEntityFactoryProvider entityFactoryProvider,
			CommandQueue commandQueue)
		{
			Configuration = configuration;
			EntityRegistry = entityRegistry;
			MatcherProvider = matcherProvider;
			SystemRegistry = systemRegistry;
			// signal the component registry that a new entity has been populated
			EntityFactoryProvider = entityFactoryProvider;
			CommandQueue = commandQueue;
		}

		/// <summary>
		/// Perform initialization operations,
		/// this includes ISystemRegistry.Initialize which activates all of the IInitializingSystem(s)
		/// </summary>
		public virtual void Initialize()
		{
			SystemRegistry.Initialize();
		}

		#region System access

		public bool TryGetSystem<TSystem>(out TSystem system)
			where TSystem : class, ISystem
		{
			return SystemRegistry.TryGetSystem<TSystem>(out system);
		}

		public IEnumerable<TSystem> GetSystems<TSystem>()
			where TSystem : class, ISystem
		{
			return SystemRegistry.GetSystems<TSystem>();
		}

		internal bool TryCreateEntityFromArchetype(string archetype, out Entity entity)
		{
			return EntityFactoryProvider.TryCreateEntityFromArchetype(archetype, out entity);
		}

		// TODO: I dont think this should be a permanent member of the ECS class as it forces you into using the command system and a specific tick behaviour
		public void EnqueueCommand(ICommand command)
		{
			CommandQueue.EnqueueCommand(command);
		}

		#endregion

		public void Dispose()
		{
			if (_disposed == false)
			{
				
			}
		}

		public Tick Tick()
		{
			var commands = CommandQueue.Flush();

			OnCommandsProcessed(commands);
			SystemRegistry.Tick(++_currentTick);

			return new Tick()
			{
				CommandQueue = commands,
				CurrentTick = _currentTick
			};
		}

		protected virtual void OnCommandsProcessed(IEnumerable<ICommand> commands)
		{
			CommandsProcessed?.Invoke(commands, _currentTick);
		}
	}
}
