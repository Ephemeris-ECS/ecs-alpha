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

		public Archetype Ancestor { get; set; }

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



}
