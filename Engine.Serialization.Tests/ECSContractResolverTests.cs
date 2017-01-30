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

			container.Bind<IComponentRegistry>().To<ComponentRegistry>().AsSingle();
			container.BindFactory<Entity, Entity.Factory>();
			container.Bind<EntityDictionary>().AsSingle();

			container.Bind<IEntityRegistry>().To<EntityRegistry>().AsSingle();

			container.Bind<ECSContractResolver>().AsSingle();
			var resolver = container.Resolve<ECSContractResolver>();
			resolver.TrackDeserializedEntities = false;
			var settings = new EntityDictionarySerializerSettings
			{
				ContractResolver = resolver,
			};

			container.BindInstance(settings).AsSingle();

			var entityDictionary = container.Resolve<EntityDictionary>();

			var json = "{ 1: { Id: 1 } }";

			var dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict, Is.EqualTo(entityDictionary));
			Assert.That(dict.Count, Is.EqualTo(1));

			json = "{ 2: { Id: 2 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict.Count, Is.EqualTo(2));
			Assert.That(dict.ContainsKey(2));

			var entity2 = dict[2];

			json = "{ 1: { Id: 1 }, 3: { Id: 3 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict.Count, Is.EqualTo(3));
			Assert.That(dict.ContainsKey(3));

			Assert.That(dict[3].Id, Is.EqualTo(3));
			Assert.That(dict[1], Is.Not.EqualTo(entity2));
			Assert.That(dict[2], Is.EqualTo(entity2));
			Assert.That(dict[3], Is.Not.EqualTo(entity2));

			entity2.AddComponent(new TestComponent());

			json = "{ 4: { Id: 4 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[4].Id, Is.EqualTo(4));
			Assert.That(dict[2].Components.Count, Is.EqualTo(1));

			json = JsonConvert.SerializeObject(dict, settings);

			json = "{ \"5\": { \"Id\": 5, \"Components\": { \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\": { \"$type\": \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests\", \"Value\": 1 } } } }";

			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[5].Id, Is.EqualTo(5));
			Assert.That(dict[5].Components.Count, Is.EqualTo(1));
			Assert.That(dict[5].Components.ContainsKey(typeof(TestComponent)), Is.True);
			Assert.That(dict[5].Components[typeof(TestComponent)] as TestComponent, Is.Not.Null);
			Assert.That((dict[5].Components[typeof(TestComponent)] as TestComponent).Value, Is.EqualTo(1));
		}

		[Test]
		public void TestDictionaryResolutionWithEntityPruning()
		{
			var container = new DiContainer();

			container.Bind<IComponentRegistry>().To<ComponentRegistry>().AsSingle();
			container.BindFactory<Entity, Entity.Factory>();
			container.Bind<EntityDictionary>().AsSingle();

			container.Bind<IEntityRegistry>().To<EntityRegistry>().AsSingle();

			container.Bind<ECSContractResolver>().AsSingle();
			var resolver = container.Resolve<ECSContractResolver>();
			var settings = new EntityDictionarySerializerSettings
			{
				ContractResolver = resolver,
			};

			container.BindInstance(settings).AsSingle();

			var entityDictionary = container.Resolve<EntityDictionary>();

			var json = "{ 1: { Id: 1 } }";

			var dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict, Is.EqualTo(entityDictionary));
			Assert.That(dict.Count, Is.EqualTo(1));

			json = "{ 2: { Id: 2 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict.Count, Is.EqualTo(1));

			var entity2 = dict[2];

			json = "{ 3: { Id: 3 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict.Count, Is.EqualTo(1));
			Assert.That(dict.ContainsKey(3));
			Assert.That(dict[3].Id, Is.EqualTo(3));
			Assert.That(dict[3], Is.Not.EqualTo(entity2));

			var entity3 = dict[3];
			entity3.AddComponent(new TestComponent());

			json = "{ 3: { Id: 3 }, 4: { Id: 4 } }";
			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[4].Id, Is.EqualTo(4));
			Assert.That(dict[3].Components.Count, Is.EqualTo(1));

			json = JsonConvert.SerializeObject(dict, settings);

			json = "{ \"5\": { \"Id\": 5, \"Components\": { \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null\": { \"$type\": \"Engine.Serialization.Tests.ECSContractResolverTests+TestComponent, Engine.Serialization.Tests\", \"Value\": 1 } } } }";

			dict = JsonConvert.DeserializeObject<EntityDictionary>(json, settings);

			Assert.That(dict[5].Id, Is.EqualTo(5));
			Assert.That(dict[5].Components.Count, Is.EqualTo(1));
			Assert.That(dict[5].Components.ContainsKey(typeof(TestComponent)), Is.True);
			Assert.That(dict[5].Components[typeof(TestComponent)] as TestComponent, Is.Not.Null);
			Assert.That((dict[5].Components[typeof(TestComponent)] as TestComponent).Value, Is.EqualTo(1));
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
