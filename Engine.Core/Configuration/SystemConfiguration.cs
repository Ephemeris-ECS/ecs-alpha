using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Systems;

namespace Engine.Configuration
{
	public abstract class SystemConfiguration
	{
		public abstract Type Type { get; }

		public SystemExtensionConfiguration[] ExtensionConfiguration { get; set; }

		// TODO: there is probably a better way to do this
		public Action BindingInitialize { get; set; }

		protected SystemConfiguration()
			: this (null)
		{
			
		}

		protected SystemConfiguration(SystemExtensionConfiguration[] extensionConfiguration)
		{
			ExtensionConfiguration = extensionConfiguration ?? new SystemExtensionConfiguration[0];
		}

		public void OnBindingInitialize()
		{
			BindingInitialize?.Invoke();
		}
	}

	public sealed class SystemConfiguration<TSystem> : SystemConfiguration
		where TSystem : ISystem
	{
		public override Type Type => typeof(TSystem);
	}

	public abstract class SystemExtensionConfiguration
	{
		public abstract Type Type { get; }

		public bool AllOfType { get; set; }

		public SystemExtensionImplementation[] Implementations { get; set; }
	}

	public abstract class SystemExtensionImplementation
	{
		public abstract Type Type { get; }
	}

	public sealed class SystemExtensionConfiguration<TExtension> : SystemExtensionConfiguration
	{

		public sealed class SystemExtensionImplementation<TImplmentation> : SystemExtensionImplementation
			where TImplmentation : TExtension
		{
			public override Type Type => typeof(TImplmentation);
		}

		public override Type Type => typeof(TExtension);
	}
}
