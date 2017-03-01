using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Archetypes
{
	public class Archetype
	{
		// TODO: possibly add categories?

		public string Name { get; }

		public Dictionary<Type, ComponentBinding> Components { get; }

		#region Constrcutors

		public Archetype(string name)
		{
			Name = name;
			Components = new Dictionary<Type, ComponentBinding>();
		}

		#endregion
	}

	/// <summary>
	/// Fluent configuration extensions
	/// </summary>
	public static class ArchetypeExtensions
	{
		public static Archetype HasComponent(this Archetype archetype, ComponentBinding componentBinding)
		{
			archetype.Components[componentBinding.GetType()] = componentBinding;
			return archetype;
		}

		public static Archetype HasComponents(this Archetype archetype, IEnumerable<ComponentBinding> componentBindings)
		{
			foreach (var componentBinding in componentBindings)
			{
				archetype.HasComponent(componentBinding);
			}
			return archetype;
		}

		// TODO: make this return the new archetpye object and copy props from parent
		public static Archetype Extends(this Archetype archetype, Archetype otherArchetype)
		{
			return archetype.HasComponents(otherArchetype.Components.Values);
		}
	}
}
