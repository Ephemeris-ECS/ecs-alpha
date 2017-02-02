using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Serialization;
using Engine.Systems;
using Engine.Testing;
using NUnit.Framework;
using Zenject;

namespace Engine.Startup.Tests
{
	[TestFixture]
	// ReSharper disable once InconsistentNaming
	public partial class ECSInstallerTests
	{
		#region test classes

		#region systems

		#region A

		public interface ISystemA : ISystem
		{
			ISystemB SystemB { get; }
		}

		public class SystemA : Systems.System, ISystemA
		{
			public ISystemB SystemB { get; }

			public IList<ISystemAExtension> Extensions { get; }

			public SystemA(IMatcherProvider matcherProvider, IEntityRegistry entityRegistry, ISystemB systemB, List<ISystemAExtension> extensions)
				: base(matcherProvider, entityRegistry)
			{
				SystemB = systemB;
				SystemB.Value = "TEST";
				Extensions = extensions;
			}

		}

		public interface ISystemAExtension : ISystemExtension
		{
			
		}

		public class ConcreteAExtensionA : ISystemAExtension
		{
			
		}

		public class ConcreteAExtensionB : ISystemAExtension
		{

		}

		#endregion

		#region B

		public interface ISystemB : ISystem
		{
			string Value { get; set; }
		}

		public class SystemB : Systems.System, ISystemB
		{
			public IList<ISystemBExtension> Extensions { get; }

			public SystemB(IMatcherProvider matcherProvider, 
				IEntityRegistry entityRegistry, 
				[InjectOptional] List<ISystemBExtension> extensions)
				: base(matcherProvider, entityRegistry)
			{
				Extensions = extensions;
			}

			public string Value { get; set; }
		}

		public interface ISystemBExtension : ISystemExtension
		{

		}

		#endregion

		#region C

		public interface ISystemC : ISystemA
		{

		}

		public class SystemC : Systems.System, ISystemC
		{
			public ISystemB SystemB { get; }

			public SystemC(IMatcherProvider matcherProvider, 
				IEntityRegistry entityRegistry, 
				ISystemB systemB)
				: base(matcherProvider, entityRegistry)
			{
				SystemB = systemB;
			}

		}


		#endregion

		#endregion

		#endregion


		[Test]
		public void TestSystemConfigurationInstaller()
		{
			var systemConfigurations = new List<SystemConfiguration>()
			{
				new SystemConfiguration<SystemA>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[]
					{
						new SystemExtensionConfiguration<ISystemAExtension>()
						{
							AllOfType = true,
						},
					}
				},
				new SystemConfiguration<SystemB>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[]
					{
						new SystemExtensionConfiguration<ISystemBExtension>()
						{
							AllOfType = true,
						},
					}
				},
				new SystemConfiguration<SystemC>()
				{
					ExtensionConfiguration = new SystemExtensionConfiguration[0]
				},
			};

			var configuration = new ECSConfiguration(null, systemConfigurations);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			Assert.That(ecs, Is.Not.Null);
			Assert.That(ecs.GetSystems<ISystemA>(), Is.Not.Null);
			Assert.That(ecs.GetSystems<ISystemA>().Count, Is.EqualTo(2));
			ISystemB iSystemB;
			Assert.That(ecs.TryGetSystem<ISystemB>(out iSystemB));
			ISystemC iSystemC;
			Assert.That(ecs.TryGetSystem<ISystemC>(out iSystemC));
			SystemA systemA;
			Assert.That(ecs.TryGetSystem<SystemA>(out systemA));
			Assert.That(systemA.SystemB, Is.Not.Null);
			Assert.That(systemA.SystemB, Is.EqualTo(iSystemB));
			Assert.That(systemA.SystemB, Is.EqualTo(iSystemC.SystemB));
			SystemB systemB;
			Assert.That(ecs.TryGetSystem<SystemB>(out systemB));
			Assert.That(systemA.Extensions.Count, Is.EqualTo(2));
			Assert.That(systemB.Extensions.Count, Is.EqualTo(0));
		}

	}
}
