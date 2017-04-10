using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Commands;
using Engine.Common.Logging;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Logging;
using Engine.Serialization;
using Engine.Systems;
using ModestTree;
using Zenject;

namespace Engine.Startup
{
	// ReSharper disable once InconsistentNaming
	public abstract class ECSInstaller<TInstaller> : Installer<TInstaller>
		where TInstaller : ECSInstaller<TInstaller>
	{
		protected ECSConfiguration Configuration;

		public EntityStateSerializer Converter => Container.Resolve<EntityStateSerializer>();

		#region Constructor

		protected ECSInstaller(ECSConfiguration configuration)
		{
			Configuration = configuration;
		}

		#endregion

		public override void InstallBindings()
		{
			// TODO: llogger shouldnt be injected like this!
			Container.BindInstance<ILogger>(new DummyLogger());

			Container.BindFactory<Entity, Entity.Factory>();

			Container.Bind<EntityDictionary>().AsSingle();

			Container.Bind<IEntityRegistry>()
				.To<EntityRegistry>()
				.AsSingle();
			
			Container.Bind<IMatcherProvider>()
				.To<MatcherProvider>()
				.AsSingle();

			Container.Bind<ISystemRegistry>()
				.To<SystemRegistry>()
				.AsSingle();


			Container.Bind<IEntityFactoryProvider>().To<EntityFactoryProvider>().AsSingle();
			
			foreach (var systemConfiguration in Configuration.Systems)
			{
				InstallSystemBinding(systemConfiguration);
			}

			Container.Bind<CommandQueue>().AsSingle();

			foreach (var archetypeConfiguration in Configuration.Archetypes)
			{
				InstallArchetype(archetypeConfiguration);
			}

			Container.Bind<EntityStateSerializer>().AsSingle();

			OnInstallBindings();
		}

		protected virtual void OnInstallBindings()
		{
		}

		public void InstallSystemBinding(SystemConfiguration systemConfiguration)
		{
			Container.BindInterfacesAndSelfTo(systemConfiguration.Type).AsSingle();
			systemConfiguration.OnBindingInitialize();
			InstallSystem(Container, systemConfiguration);
		}

		private static void InstallSystem(DiContainer container, SystemConfiguration systemConfiguration)
		{
			if (systemConfiguration.ExtensionConfiguration != null)
			{
				//container.BindAllInterfaces(systemConfiguration.Type).To(systemConfiguration.Type).AsSingle();
				foreach (var extensionConfiguration in systemConfiguration.ExtensionConfiguration)
				{
					if (extensionConfiguration.AllOfType)
					{
						container.Bind(extensionConfiguration.Type).To(t => t.AllNonAbstractClasses().DerivingFrom(extensionConfiguration.Type)).AsTransient();
					}
					else
					{
						foreach (var extensionImplementation in extensionConfiguration.Implementations)
						{
							container.Bind(extensionConfiguration.Type).To(extensionImplementation.Type).AsTransient();
						}
					}
				}
			}
		}

		private void InstallArchetype(Archetype archetypeConfiguration)
		{
			Container.Bind<IEntityFactory>() 
				.FromSubContainerResolve()
				.ByMethod(container => InstallArchetypeFactory(container, archetypeConfiguration))
				.AsSingle()
				.NonLazy();
		}

		private static void InstallArchetypeFactory(DiContainer container, Archetype archetype)
		{
			container.Bind<IEntityFactory>().To<EntityFactory>().AsSingle();
			container.BindInstance(archetype).AsSingle();
			foreach (var componentBinding in archetype.Components.Values)
			{
				container.Bind(componentBinding.ComponentType).AsTransient();
			}
		}
	}

	// ReSharper disable InconsistentNaming
	public abstract class ECSInstaller<TECS, TConfiguration, TInstaller, TECSRoot, TScenario> : ECSInstaller<TInstaller>
		where TECS : ECS<TConfiguration>
		where TConfiguration : ECSConfiguration
		where TInstaller : ECSInstaller<TInstaller>
		where TECSRoot : ECSRoot<TECS, TConfiguration>
		where TScenario : Scenario<TECS, TConfiguration>
		// ReSharper restore InconsistentNaming
	{
		protected ECSInstaller(ECSConfiguration configuration)
			: base(configuration)
		{
		}

		public override void InstallBindings()
		{
			Container.Bind(typeof(TECS)).AsSingle();
			base.InstallBindings();
		}

		// ReSharper disable once InconsistentNaming
		protected static TECSRoot CreateECSRoot(TScenario scenario, DiContainer container)
		{
			Assert.IsNotNull(scenario, nameof(scenario));
			Assert.IsNotNull(scenario.Configuration, nameof(scenario.Configuration));

			container = container ?? new DiContainer();

			// TODO: there must be an easier way to bind something polymorphically
			// ie. I want both the concrete and base types resolving to the same instance
			container.Bind<Scenario<TECS, TConfiguration>>().FromInstance(scenario);
			container.BindInstance<TScenario>(scenario);

			// TODO: there must be an easier way to bind something polymorphically
			// ie. I want both the concrete and base types resolving to the same instance
			container.Bind<ECSConfiguration>().FromInstance(scenario.Configuration);
			container.BindInstance(scenario.Configuration);

			Install(container);

			return container.Instantiate<TECSRoot>();
		}
	}
}
