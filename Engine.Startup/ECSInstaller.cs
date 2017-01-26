using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Archetypes;
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
			
			Container.Bind<IComponentRegistry>()
				.To<ComponentRegistry>()
				.AsSingle();

			Container.Bind<ISystemRegistry>()
				.To<SystemRegistry>()
				.AsSingle();
			
			foreach (var systemConfiguration in Configuration.Systems)
			{
				InstallSystemBinding(systemConfiguration);
			}

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
			Container.Bind(systemConfiguration.Type).AsSingle();
			Container.BindAllInterfaces(systemConfiguration.Type).To(systemConfiguration.Type)
				//.FromSubContainerResolve().ByMethod(container => InstallSystem(container, systemConfiguration))
				//// TODO: perhaps decide if it should be a singleton in configuration
				.AsSingle();
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
						container.Bind(extensionConfiguration.Type).To(t => t.AllNonAbstractClasses().DerivingFrom(extensionConfiguration.Type));
					}
					else
					{
						foreach (var extensionImplementation in extensionConfiguration.Implementations)
						{
							container.Bind(extensionConfiguration.Type).To(extensionImplementation);
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
			foreach (var componentBinding in archetypeConfiguration.Components)
			{
				container.Bind<IComponent>().To(componentBinding.ComponentType);
			}
		}
	}

	// ReSharper disable InconsistentNaming
	public abstract class ECSInstaller<TECS, TConfiguration, TInstaller, TECSRoot> 
		: ECSInstaller<TInstaller>
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

			container.BindInstance(configuration);
			Install(container);

			return container.Instantiate<TECSRoot>();
		}
	}
}
