
using System.ComponentModel;
using Engine.Configuration;
using Engine.Startup;
using Zenject;

namespace Engine.Testing
{
	public class TestInstaller : ECSInstaller<TestECS, ECSConfiguration, TestInstaller, ECSRoot<TestECS, ECSConfiguration>>
	{
		public DiContainer PublicContainer => Container;

		public TestInstaller(ECSConfiguration configuration)
			: base(configuration)
		{
		}

		public static ECSRoot<TestECS, ECSConfiguration> CreatTestRoot(ECSConfiguration configuration)
		{
			return CreateECSRoot(configuration, null);
		}
	}
}