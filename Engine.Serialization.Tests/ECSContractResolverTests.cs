using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Entities;
using Newtonsoft.Json;
using NUnit.Framework;
using Zenject;

namespace Engine.Serialization.Tests
{
	// ReSharper disable once InconsistentNaming
	[TestFixture]
	public class ECSContractResolverTests
	{
		public class TestComponent : IComponent
		{
			public int Value { get; set; }
		}

		[Test]
		public void TestDictionaryResolution()
		{
			var container = new DiContainer();

			container.Bind<ECSContractResolver>().AsSingle();
			container.Bind<EntityDictionarySerializerSettings>().AsSingle();
			container.Bind<EntityDictionary>().AsSingle();

			var entityDictionary = container.Resolve<EntityDictionary>();

			var json = "{ 1: { Id: 1 } }";

			var settings = container.Resolve<EntityDictionarySerializerSettings>();
			var dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict, Is.EqualTo(entityDictionary));
			Assert.That(dict.Count, Is.EqualTo(1));

			json = "{ 2: { Id: 2 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict.Count, Is.EqualTo(2));

			var entity2 = dict[2];

			json = "{ 2: { Id: 3 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[2].Id, Is.EqualTo(3));
			Assert.That(dict[2], Is.EqualTo(entity2));

			entity2.AddComponent(new TestComponent());

			json = "{ 2: { Id: 4 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[2].Id, Is.EqualTo(4));
			Assert.That(dict[2].Components.Count, Is.EqualTo(1));

			json = JsonConvert.SerializeObject(dict, settings);

			json = "{ \"2\": { \"Id\": 5, \"Components\": { \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\": { \"$type\": \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests\", \"Value\": 1 } } } }";

			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[2].Id, Is.EqualTo(5));
			Assert.That(dict[2].Components.Count, Is.EqualTo(1));
			Assert.That(dict[2].Components.ContainsKey(typeof(TestComponent)), Is.True);
			Assert.That(dict[2].Components[typeof(TestComponent)] as TestComponent, Is.Not.Null);
			Assert.That((dict[2].Components[typeof(TestComponent)] as TestComponent).Value, Is.EqualTo(1));
		}

		[Test]
		public void TestDictionaryResolutionWithEntityPruning()
		{
			var container = new DiContainer();

			container.Bind<ECSContractResolver>().AsSingle();
			container.Bind<EntityDictionarySerializerSettings>().AsSingle();
			container.Bind<EntityDictionary>().AsSingle();

			var entityDictionary = container.Resolve<EntityDictionary>();

			var json = "{ 1: { Id: 1 } }";

			var settings = container.Resolve<EntityDictionarySerializerSettings>();
			var dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict, Is.EqualTo(entityDictionary));
			Assert.That(dict.Count, Is.EqualTo(1));

			json = "{ 2: { Id: 2 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict.Count, Is.EqualTo(2));

			var entity2 = dict[2];

			json = "{ 2: { Id: 3 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[2].Id, Is.EqualTo(3));
			Assert.That(dict[2], Is.EqualTo(entity2));

			entity2.AddComponent(new TestComponent());

			json = "{ 2: { Id: 4 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[2].Id, Is.EqualTo(4));
			Assert.That(dict[2].Components.Count, Is.EqualTo(1));

			json = JsonConvert.SerializeObject(dict, settings);

			json = "{ \"2\": { \"Id\": 5, \"Components\": { \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\": { \"$type\": \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests\", \"Value\": 1 } } } }";

			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[2].Id, Is.EqualTo(5));
			Assert.That(dict[2].Components.Count, Is.EqualTo(1));
			Assert.That(dict[2].Components.ContainsKey(typeof(TestComponent)), Is.True);
			Assert.That(dict[2].Components[typeof(TestComponent)] as TestComponent, Is.Not.Null);
			Assert.That((dict[2].Components[typeof(TestComponent)] as TestComponent).Value, Is.EqualTo(1));
		}

		//[Test]
		//public void TestPopulation()
		//{
		//	var container = new DiContainer();
		//	container.Bind<EntityDictionary>().AsSingle();
		//	var entityDictionary = container.Resolve<EntityDictionary>();

		//	var json = "{ 1: { Id: 1 } }";

		//	JsonConvert.PopulateObject(json, entityDictionary);

		//	Assert.That(entityDictionary.Count, Is.EqualTo(1));

		//	json = "{ 2: { Id: 2 } }";
		//	JsonConvert.PopulateObject(json, entityDictionary);

		//	Assert.That(entityDictionary.Count, Is.EqualTo(2));

		//	var entity2 = entityDictionary[2];

		//	json = "{ 2: { Id: 3 } }";
		//	JsonConvert.PopulateObject(json, entityDictionary);

		//	Assert.That(entityDictionary[2].Id, Is.EqualTo(3));
		//	Assert.That(entityDictionary[2], Is.EqualTo(entity2)); // this is the problem, a new entity is constructed and replaced
		//}
	}
}
