using System;
using System.Collections.Generic;
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
	}
}
