using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Archetypes
{
	public abstract class ComponentBinding
	{
		public abstract Type ComponentType { get; }

		/// <summary>
		/// represents the minimal set of property overrides to initialize the component in the required initial state.
		/// </summary>
		public string ComponentTemplateSerialized { get; }
		
		protected ComponentBinding()
		{
			ComponentTemplateSerialized = "{}";
		}

		protected ComponentBinding(string componentTemplateSerialized)
		{
			ComponentTemplateSerialized = componentTemplateSerialized;
		}
	}

	public class ComponentBinding<TComponent> : ComponentBinding
		where TComponent : IComponent
	{
		public override Type ComponentType => typeof(TComponent);

		///// <summary>
		///// represents the initial state of the component
		///// values will be copied to new instances where they do not match
		///// if this is null the compont template will be deserialized from <cref:ComponentTemplateSerialized />
		///// </summary>
		//public TComponent ComponentTemplate { get; set; }
	}
}
