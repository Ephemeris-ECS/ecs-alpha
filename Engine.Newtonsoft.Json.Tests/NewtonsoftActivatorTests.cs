using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Zenject;

namespace Engine.Newtonsoft.Json.Tests
{
	[TestFixture]
	public class NewtonsoftActivatorTests
	{
		public class ActivatorTestA
		{
			public ActivatorTestA()
			{
			}
		}

		public class ActivatorTestB
		{
			public ActivatorTestA TestA { get; }

			public ActivatorTestB(ActivatorTestA testA)
			{
				TestA = testA;
			}
		}

		//TODO this just validates that resolving from the DI container is a valid option;
		public class ActivatorTestContractResolver : DefaultContractResolver
		{
			private readonly DiContainer _container;

			public ActivatorTestContractResolver(DiContainer container)
			{
				_container = container;
			}

			public override JsonContract ResolveContract(Type type)
			{
				var contract = base.ResolveContract(type);

				contract.DefaultCreator = () => DefaultCreator(type);

				return contract;
			}

			private object DefaultCreator(Type type)
			{
				return _container.Instantiate(type);
			}
		}

		[Test]
		public void TestDeserializeEmpty()
		{
			const string json = "{}";

			var test = JsonConvert.DeserializeObject<ActivatorTestA>(json);
			Assert.That(test, Is.Not.Null);
		}

		[Test]
		public void TestDeserializeFromContainer()
		{
			var container = new DiContainer();
			container.Bind<ActivatorTestA>();
			
			const string json = "{}";

			var contractResolver = new ActivatorTestContractResolver(container);
			var settings = new JsonSerializerSettings()
			{
				ContractResolver = contractResolver
			};

			var test = JsonConvert.DeserializeObject<ActivatorTestA>(json, settings);
			Assert.That(test, Is.Not.Null);
		}

		[Test]
		public void TestDeserializeFromContainerWithDependency()
		{
			var container = new DiContainer();
			container.Bind<ActivatorTestA>();
			container.Bind<ActivatorTestB>();

			const string json = "{}"; 

			var contractResolver = new ActivatorTestContractResolver(container);
			var settings = new JsonSerializerSettings()
			{
				ContractResolver = contractResolver
			};

			var test = JsonConvert.DeserializeObject<ActivatorTestB>(json, settings);
			Assert.That(test, Is.Not.Null);
			Assert.That(test.TestA, Is.Not.Null);
		}

		[Test]
		public void TestDeserializeFromContainerWithDependencyAndPropertyOverride()
		{
			var container = new DiContainer();
			container.Bind<ActivatorTestA>();
			container.Bind<ActivatorTestB>();

			const string json = "{ TestA: null }";

			var contractResolver = new ActivatorTestContractResolver(container);
			var settings = new JsonSerializerSettings()
			{
				ContractResolver = contractResolver
			};

			var test = JsonConvert.DeserializeObject<ActivatorTestB>(json, settings);
			Assert.That(test, Is.Not.Null);
			Assert.That(test.TestA, Is.Null);
		}
	}
}
