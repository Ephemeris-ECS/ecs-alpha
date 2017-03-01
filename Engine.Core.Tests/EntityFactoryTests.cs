using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Engine.Archetypes;
using Engine.Configuration;
using Engine.Entities;
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
						{ typeof(ComponentBinding<TestComponentA>), new ComponentBinding<TestComponentA>() },
					}
				},
				new Archetype("TestB")
				{
					Components =
					{
						{ typeof(ComponentBinding<TestComponentB>),  new ComponentBinding<TestComponentB>() },
					}
				},
			};
			
			var configuration = new ECSConfiguration(archetypes, null, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			Entity entity;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entity));

			Entity entityA;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entityA));
			Assert.That(entityA.GetComponent<TestComponentA>(), Is.Not.Null);
			Assert.That(entityA.HasComponent<TestComponentB>(), Is.False);

			Entity entityB;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestB", out entityB));
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
						{
							typeof(ComponentBinding<TestComponentA>),
							new ComponentBinding<TestComponentA>()
							{
								ComponentTemplateSerialized = "{ \"StringValuePublic\": \"Woo\", \"_stringValuePrivate\": \"yay\", \"IntValuePublic\": \"1\", \"_intValuePrivate\": \"2\" }"
							}
						},
					}
				},
				new Archetype("TestB")
				{
					Components =
					{
						{
							typeof(ComponentBinding<TestComponentB>),
							new ComponentBinding<TestComponentB>()
							{
								ComponentTemplateSerialized = "{ \"IntDictionaryValuePublic\": { \"1\": \"1\", \"2\": \"2\" } }"
							}
						},
					}
				},
			};


			var configuration = new ECSConfiguration(archetypes, null, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			Entity entity;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entity));

			Entity entityA;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entityA));
			TestComponentA componentA;
			Assert.That(entityA.TryGetComponent<TestComponentA>(out componentA), Is.True);
			Assert.That(componentA.StringValuePublic, Is.EqualTo("Woo"));
			Assert.That(componentA.IntValuePublic, Is.EqualTo(1));

			// TODO: test private members via reflection
			// TODO: decide if private members should be settable in the template, or if it should all be public

			TestComponentB componentB;
			Entity entityB;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestB", out entityB));
			Assert.That(entityB.TryGetComponent<TestComponentB>(out componentB), Is.True);
			Assert.That(componentB.IntDictionaryValuePublic.Count, Is.EqualTo(2));
			Assert.That(componentB.IntDictionaryValuePublic.Keys.Contains(1));
			Assert.That(componentB.IntDictionaryValuePublic[1], Is.EqualTo(1));
		}

		[Test]
		public void TestEntityTemplate()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("TestA")
				{
					Components =
					{
						{
							typeof(ComponentBinding<TestComponentA>),
							new ComponentBinding<TestComponentA>()
							{
								ComponentTemplate = new TestComponentA()
								{
									StringValuePublic = "Woo",
									IntValuePublic = 1,
								}
							}
						},
					}
				},
				new Archetype("TestB")
				{
					Components =
					{
						{
							typeof(ComponentBinding<TestComponentB>),
							new ComponentBinding<TestComponentB>()
							{
								ComponentTemplate = new TestComponentB()
								{
									IntDictionaryValuePublic = new Dictionary<int, int>()
									{
										{ 1, 1 },
										{ 2, 2 }
									}
								}
							}
						},
					}
				},
			};


			var configuration = new ECSConfiguration(archetypes, null, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			Entity entity;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entity));

			Entity entityA;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entityA));
			TestComponentA componentA;
			Assert.That(entityA.TryGetComponent<TestComponentA>(out componentA), Is.True);
			Assert.That(componentA.StringValuePublic, Is.EqualTo("Woo"));
			Assert.That(componentA.IntValuePublic, Is.EqualTo(1));

			TestComponentB componentB;
			Entity entityB;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestB", out entityB));
			Assert.That(entityB.TryGetComponent<TestComponentB>(out componentB), Is.True);
			Assert.That(componentB.IntDictionaryValuePublic.Count, Is.EqualTo(2));
			Assert.That(componentB.IntDictionaryValuePublic.Keys.Contains(1));
			Assert.That(componentB.IntDictionaryValuePublic[1], Is.EqualTo(1));
		}


		//[Test]
		public void TestEntityTemplateDeserializationTime()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("TestA")
				{
					Components =
					{
						{
							typeof(ComponentBinding<TestComponentA>),
							new ComponentBinding<TestComponentA>()
							{
								ComponentTemplateSerialized = "{ \"StringValuePublic\": \"Woo\", \"_stringValuePrivate\": \"yay\", \"IntValuePublic\": \"1\", \"_intValuePrivate\": \"2\" }"
							}
						},
					}
				},
				new Archetype("TestB")
				{
					Components =
					{
						{
							typeof(ComponentBinding<TestComponentB>),
							new ComponentBinding<TestComponentB>()
							{
								ComponentTemplateSerialized = "{ \"IntDictionaryValuePublic\": { \"1\": \"1\", \"2\": \"2\" } }"
							}
						},
					}
				},
			};

			var configuration = new ECSConfiguration(archetypes, null, null);

			var ecs = TestInstaller.CreatTestRoot(configuration).ECS;

			Entity entity;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entity));

			const int repeat = 1000;

			var stopwatch = new Stopwatch();
			Entity entityA;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entityA));
			stopwatch.Start();

			for (var i = 0; i < repeat; i++)
			{
				Assert.That(ecs.TryCreateEntityFromArchetype("TestA", out entityA));
				TestComponentA componentA;
				Assert.That(entityA.TryGetComponent<TestComponentA>(out componentA), Is.True);
				Assert.That(componentA.StringValuePublic, Is.EqualTo("Woo"));
				Assert.That(componentA.IntValuePublic, Is.EqualTo(1));
			}
			var elapsedd = stopwatch.ElapsedMilliseconds;



			TestComponentB componentB;
			Entity entityB;
			Assert.That(ecs.TryCreateEntityFromArchetype("TestB", out entityB));
			Assert.That(entityB.TryGetComponent<TestComponentB>(out componentB), Is.True);
			Assert.That(componentB.IntDictionaryValuePublic.Count, Is.EqualTo(2));
			Assert.That(componentB.IntDictionaryValuePublic.Keys.Contains(1));
			Assert.That(componentB.IntDictionaryValuePublic[1], Is.EqualTo(1));
		}
	}
}
