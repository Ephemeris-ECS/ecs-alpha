using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Engine.Components;
using Engine.Serialization;
using Engine.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Engine.Archetypes
{
	public abstract class ComponentBinding
	{
		public abstract Type ComponentType { get; }
	}

	public abstract class CreateComponentBinding : ComponentBinding
	{
		protected static readonly JsonSerializer ComponentTemplateSerializer;

		static CreateComponentBinding()
		{
			// TODO: extract this all to the serialization assembly
			var serializerSettings = new JsonSerializerSettings()
			{
				ContractResolver = new IncludePrivateMembersContractResolver(),
				TypeNameHandling = TypeNameHandling.All,
			};
			serializerSettings.Converters.Add(new StringEnumConverter());

			// TODO: we probably need some settings overrides
			ComponentTemplateSerializer = JsonSerializer.CreateDefault(serializerSettings);
		}

		/// <summary>
		/// represents the minimal set of property overrides to initialize the component in the required initial state.
		/// </summary>
		public string ComponentTemplateSerialized { get; set; }

		public abstract void InitialiseTemplate();

		public abstract void PopulateComponent(IComponent component);

		#region constrcutors

		protected CreateComponentBinding()
		{
			ComponentTemplateSerialized = "{}";
		}

		protected CreateComponentBinding(string componentTemplateSerialized)
		{
			ComponentTemplateSerialized = componentTemplateSerialized;
		}

		#endregion
	}

	public class ComponentBinding<TComponent> : CreateComponentBinding
		where TComponent : IComponent
	{
		private class TemplateValueProxy
		{
			private readonly Func<TComponent, object> _getter;
			private readonly Action<TComponent, object> _setter;

			public TemplateValueProxy(Func<TComponent, object> getter, Action<TComponent, object> setter)
			{
				_getter = getter;
				_setter = setter;
			}

			public void CopyValue(TComponent source, TComponent destination)
			{
				_setter(destination, _getter(source));
			}
		}

		private List<TemplateValueProxy> _templateValueProxies;

		public override Type ComponentType => typeof(TComponent);

		/// <summary>
		/// represents the initial state of the component
		/// values will be copied to new instances where they do not match
		/// if this is null the compont template will be deserialized from <cref:ComponentTemplateSerialized />
		/// </summary>
		public TComponent ComponentTemplate { get; set; }

		public override void InitialiseTemplate()
		{
			if (_templateValueProxies == null)
			{
				_templateValueProxies = new List<TemplateValueProxy>();

				if (ComponentTemplate == null)
				{
					using (var reader = new JsonTextReader(new StringReader(ComponentTemplateSerialized)))
					{
						ComponentTemplate = ComponentTemplateSerializer.Deserialize<TComponent>(reader);
					}
				}

				foreach (var field in typeof(TComponent).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy))
				{
					// TODO: replace with skip sync attribute, or inversely sync only attributed properties
					if (field.Name.Equals("Id"))
					{
						continue;
					}
					var getter = DynamicMethodHelper.CreateGet<TComponent>(field);
					var value = getter(ComponentTemplate);

					if (value != null && value.Equals(DefaultValueHelper.GetDefault(field.FieldType)) == false)
					{
						// TODO: replace this special case handling with something mroe generic
						// all object references aren't going to work
						if (field.FieldType.IsClass && field.FieldType != typeof(string))
						{
							var sb = new StringBuilder(256);
							var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
							using (var jsonWriter = new JsonTextWriter(sw))
							{
								ComponentTemplateSerializer.Serialize(jsonWriter, value, field.FieldType);
							}
							var valueString = sw.ToString();

							_templateValueProxies.Add(new TemplateValueProxy(obj => {
								using (var reader = new JsonTextReader(new StringReader(valueString)))
								{
									return ComponentTemplateSerializer.Deserialize(reader, field.FieldType);
								}
							}, DynamicMethodHelper.CreateSet<TComponent>(field)));
						}
						else
						{
							_templateValueProxies.Add(new TemplateValueProxy(getter, DynamicMethodHelper.CreateSet<TComponent>(field)));
						}
					}
				}
			}
		}

		public override void PopulateComponent(IComponent component)
		{
			InitialiseTemplate();
			PopulateComponent((TComponent) component);
		}

		private void PopulateComponent(TComponent component)
		{
			foreach (var templateValueProxy in _templateValueProxies)
			{
				templateValueProxy.CopyValue(ComponentTemplate, component);
			}
		}
	}

	public abstract class RemoveComponentBinding : ComponentBinding
	{
		
	}

	public class RemoveComponentBinding<TComponent> : RemoveComponentBinding
		where TComponent : IComponent
	{
		public override Type ComponentType => typeof(TComponent);

	}
}
