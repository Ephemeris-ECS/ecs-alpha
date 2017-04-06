using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Exceptions;

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

		public static implicit operator string(Archetype archetype)
		{
			return archetype.Name;
		}
	}

	/// <summary>
	/// Fluent configuration extensions
	/// </summary>
	public static class ArchetypeExtensions
	{
		public static void HasComponent(this Dictionary<Type, ComponentBinding> components, ComponentBinding componentBinding, bool overwrite = true)
		{
			if (overwrite)
			{
				components[componentBinding.GetType()] = componentBinding;
			}
			else
			{
				components.Add(componentBinding.GetType(), componentBinding);
			}
		}

		public static Archetype HasComponent(this Archetype archetype, ComponentBinding componentBinding, bool overwrite = true)
		{
			try
			{
				archetype.Components.HasComponent(componentBinding, overwrite);
			}
			catch (InvalidOperationException ioex)
			{
				throw new ConfigurationException($"Archetype '{archetype.Name}' already ahs component binding for type {componentBinding.GetType()}", ioex);
			}
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

		public static Archetype RemoveComponent<TComponent>(this Archetype archetype)
			where TComponent : IComponent
		{
			return archetype.RemoveComponent(typeof(TComponent));
		}

		public static Archetype RemoveComponent(this Archetype archetype, Type componentType)
		{
			archetype.Components.Remove(componentType);
			return archetype;
		}

	}

}
