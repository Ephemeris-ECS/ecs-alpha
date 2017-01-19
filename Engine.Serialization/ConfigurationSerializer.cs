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

		public static string SerializeConfiguration(ECSConfiguration configuration)
		{
			return SerializeObjectInternal(configuration, configuration.GetType(), _configSerializer);
		}

		public static TConfiguration DeserializeConfiguration<TConfiguration>(string json)
			where TConfiguration : ECSConfiguration
		{
			return DeserializeObject<TConfiguration>(json, _configSerializer);
		}
	}
}
