using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;
using Engine.Exceptions;

namespace Engine.Archetypes
{
	/// <summary>
	/// Fluent configuration extensions
	/// </summary>
	public static class ArchetypeExtensions
	{
		public static void HasComponent(this Dictionary<Type, ComponentBinding> components, ComponentBinding componentBinding)
		{
			components[componentBinding.GetType()] = componentBinding;
		}

		public static Archetype HasComponent<TComponent>(this Archetype archetype)
			where TComponent : IComponent
		{
			try
			{
				archetype.Components.HasComponent(new ComponentBinding<TComponent>());
			}
			catch (InvalidOperationException ioex)
			{
				throw new ConfigurationException($"Archetype '{archetype.Name}' already ahs component binding for type {typeof(TComponent)}", ioex);
			}
			return archetype;
		}

		public static Archetype HasComponent(this Archetype archetype, ComponentBinding componentBinding)
		{
			try
			{
				archetype.Components.HasComponent(componentBinding);
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
			archetype.Ancestor = otherArchetype;
			return archetype;
		}

		public static Archetype RemoveComponent<TComponent>(this Archetype archetype)
			where TComponent : IComponent
		{
			archetype.Components.HasComponent(new RemoveComponentBinding<TComponent>());
			return archetype;
		}

	}
}
