using System.Collections.Generic;
using Engine.Archetypes;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using NUnit.Framework;

namespace Engine.Testing.Components
{
	public abstract class ComponentTestBase<TComponent>
		where TComponent : class, IComponent
	{
		/// <summary>
		/// Tests that the component can be created in a default state via the ECS installer
		/// </summary>
		[Test]
		public virtual void TestComponentCreationViaArchetype()
		{
			var archetypes = new List<Archetype>()
			{
				new Archetype("Test")
				{
					Components =
					{
						{  typeof(ComponentBinding<TComponent>), new ComponentBinding<TComponent>() },
					}
				},
			};

			var configuration = new ECSConfiguration(archetypes, null, null);
			var scenario = new TestScenario() {Configuration = configuration};
			var ecs = TestInstaller.CreatTestRoot(scenario).ECS;
			Entity entity;
			ecs.TryCreateEntityFromArchetype("Test", out entity);

			var component = entity.GetComponent<TComponent>();
			Assert.That(component, Is.Not.Null);

			TestComponentCreationViaArchetype_PostCreate(component);
		}

		/// <summary>
		/// Perform any component specific default initialization tests in the subclass
		/// </summary>
		/// <param name="component">Component being tested</param>
		protected virtual void TestComponentCreationViaArchetype_PostCreate(TComponent component)
		{
			
		}
	}
}
