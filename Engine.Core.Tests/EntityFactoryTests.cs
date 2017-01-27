using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Testing;
using NUnit.Framework;

namespace Engine.Core.Tests
{
	[TestFixture]
	public class EntityFactoryTests
	{
		[Test]
		public void TestArchetypeInstallation()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("TestA")
				{
					Components =
					{
						new ComponentBinding<TestComponentA>(),
					}
				},
				new Archetype("TestB")
				{
					Components =
					{
						new ComponentBinding<TestComponentB>(),
					}
				},
			};



			var configuration = new ECSConfiguration(archetypes, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			ecs.CreateEntityFromArchetype("TestA");

			var entityA = ecs.CreateEntityFromArchetype("TestA");
			Assert.That(entityA.GetComponent<TestComponentA>(), Is.Not.Null);
			Assert.That(entityA.HasComponent<TestComponentB>(), Is.False);

			var entityB = ecs.CreateEntityFromArchetype("TestB");
			Assert.That(entityB.GetComponent<TestComponentB>(), Is.Not.Null);
			Assert.That(entityB.HasComponent<TestComponentA>(), Is.False);
		}

		[Test]
		public void TestEntityTemplateDeserialization()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("TestA")
				{
					Components =
					{
						new ComponentBinding<TestComponentA>()
						{
							ComponentTemplateSerialized = "{ \"StringValuePublic\": \"Woo\", \"_stringValuePrivate\": \"yay\", \"IntValuePublic\": \"1\", \"_intValuePrivate\": \"2\" }"
						},
					}
				},
				new Archetype("TestB")
				{
					Components =
					{
						new ComponentBinding<TestComponentB>()
						{
							ComponentTemplateSerialized = "{ \"IntDictionaryValuePublic\": { \"1\": \"1\", \"2\": \"2\" } }"
						},
					}
				},
			};



			var configuration = new ECSConfiguration(archetypes, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			ecs.CreateEntityFromArchetype("TestA");

			var entityA = ecs.CreateEntityFromArchetype("TestA");
			TestComponentA componentA;
			Assert.That(entityA.TryGetComponent<TestComponentA>(out componentA), Is.True);
			Assert.That(componentA.StringValuePublic, Is.EqualTo("Woo"));
			Assert.That(componentA.IntValuePublic, Is.EqualTo(1));

			TestComponentB componentB;
			var entityB = ecs.CreateEntityFromArchetype("TestB");
			Assert.That(entityB.TryGetComponent<TestComponentB>(out componentB), Is.True);
			Assert.That(componentB.IntDictionaryValuePublic.Count, Is.EqualTo(2));
			Assert.That(componentB.IntDictionaryValuePublic.Keys.Contains(1));
			Assert.That(componentB.IntDictionaryValuePublic[1], Is.EqualTo(1));
		}

		[Test]
		public void TestEntityTemplateDeserializationTime()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("TestA")
				{
					Components =
					{
						new ComponentBinding<TestComponentA>()
						{
							ComponentTemplateSerialized = "{ \"StringValuePublic\": \"Woo\", \"_stringValuePrivate\": \"yay\", \"IntValuePublic\": \"1\", \"_intValuePrivate\": \"2\" }"
						},
					}
				},
				new Archetype("TestB")
				{
					Components =
					{
						new ComponentBinding<TestComponentB>()
						{
							ComponentTemplateSerialized = "{ \"IntDictionaryValuePublic\": { \"1\": \"1\", \"2\": \"2\" } }"
						},
					}
				},
			};

			var configuration = new ECSConfiguration(archetypes, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			ecs.CreateEntityFromArchetype("TestA");

			const int repeat = 1000;

			var stopwatch = new Stopwatch();
			stopwatch.Start();

			for (var i = 0; i < repeat; i++)
			{
				var entityA = ecs.CreateEntityFromArchetype("TestA");
				TestComponentA componentA;
				Assert.That(entityA.TryGetComponent<TestComponentA>(out componentA), Is.True);
				Assert.That(componentA.StringValuePublic, Is.EqualTo("Woo"));
				Assert.That(componentA.IntValuePublic, Is.EqualTo(1));
			}
			var elapsedd = stopwatch.ElapsedMilliseconds;



			TestComponentB componentB;
			var entityB = ecs.CreateEntityFromArchetype("TestB");
			Assert.That(entityB.TryGetComponent<TestComponentB>(out componentB), Is.True);
			Assert.That(componentB.IntDictionaryValuePublic.Count, Is.EqualTo(2));
			Assert.That(componentB.IntDictionaryValuePublic.Keys.Contains(1));
			Assert.That(componentB.IntDictionaryValuePublic[1], Is.EqualTo(1));
		}
	}
}
