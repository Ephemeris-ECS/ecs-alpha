using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
using Engine.Commands;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
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
			// TODO: perhaps these shouldnt be here

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

		private static void InstallArchetypeFactory(DiContainer container, Archetype archetypeConfiguration)
		{
			container.Bind<IEntityFactory>().To<EntityFactory>().AsSingle();
			container.BindInstance(archetypeConfiguration).AsSingle();
			foreach (var componentBinding in archetypeConfiguration.Components.Values)
			{
				container.Bind(componentBinding.ComponentType).AsTransient();
			}
		}
	}

	// ReSharper disable InconsistentNaming
	public abstract class ECSInstaller<TECS, TConfiguration, TInstaller, TECSRoot> : ECSInstaller<TInstaller>
		where TECS : ECS<TConfiguration>
		where TConfiguration : ECSConfiguration
		where TInstaller : ECSInstaller<TInstaller>
		where TECSRoot : ECSRoot<TECS, TConfiguration>
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
		protected static TECSRoot CreateECSRoot(TConfiguration configuration, DiContainer container)
		{
			Assert.IsNotNull(configuration, nameof(configuration));

			container = container ?? new DiContainer();

			// bind as super type
			container.Bind<ECSConfiguration>().FromInstance(configuration);
			// bind as sub type
			container.BindInstance(configuration);

			Install(container);

			return container.Instantiate<TECSRoot>();
		}
	}
}
