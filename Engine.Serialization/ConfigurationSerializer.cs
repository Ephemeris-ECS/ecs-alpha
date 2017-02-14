using System;
using System.Globalization;
using System.IO;
using System.Text;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Serialization.Newtonsoft.Json;
using ICSharpCode.SharpZipLib.GZip;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using Zenject;

namespace Engine.Serialization
{
	// ReSharper disable once InconsistentNaming
	public class ConfigurationSerializer : SerializerBase
	{
		private static readonly JsonSerializer _configSerializer;

		static ConfigurationSerializer()
		{
			var configurationSerializerSettings = new ConfigurationSerializerSettings();
			_configSerializer = JsonSerializer.CreateDefault(configurationSerializerSettings);
		}

		#region config serializer

		public static string SerializeConfiguration(ECSConfiguration configuration)
		{
			return SerializeObjectInternal(configuration, configuration.GetType(), _configSerializer);
		}

		public static TConfiguration DeserializeConfiguration<TConfiguration>(string json)
			where TConfiguration : ECSConfiguration
		{
			return DeserializeObject<TConfiguration>(json, _configSerializer);
		}

		#endregion

		#region scenario
		
		public static string SerializeScenario<TScenario>(TScenario scenario)
		{
			return SerializeObjectInternal(scenario, scenario.GetType(), _configSerializer);
		}

		public static TScenario DeserializeScenario<TScenario>(string json)
			where TScenario : class
		{
			return DeserializeObject<TScenario>(json, _configSerializer);
		}

		#endregion

		#region general purpose

		public static string Serialize(object obj)
		{
			return SerializeObjectInternal(obj, obj.GetType(), _configSerializer);
		}

		public static T Deserialize<T>(string json)
			where T : class
		{
			return DeserializeObject<T>(json, _configSerializer);
		}

		#endregion
	}
}
