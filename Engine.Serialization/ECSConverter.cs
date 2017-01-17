using System;
using System.Globalization;
using System.IO;
using System.Text;
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
	public class ECSConverter
	{
		private readonly JsonSerializer _entitySerializer;

		private readonly JsonSerializer _configSerializer;

		public ECSConverter(DiContainer container, ECSSerializerSettings entitySerializerSettings)
		{
			var entityContractResolver = new ECSContractResolver(container)
			{
				TrackDeserializedEntities = entitySerializerSettings.PruneEntitiesOnDeserialize
			};
			entitySerializerSettings.ContractResolver = entityContractResolver;

			_entitySerializer = JsonSerializer.CreateDefault(entitySerializerSettings);
			_configSerializer = JsonSerializer.CreateDefault();
		}

		private static string SerializeObjectInternal<T>(T value, Type type, JsonSerializer jsonSerializer)
		{
			StringBuilder sb = new StringBuilder(256);
			StringWriter sw = new StringWriter(sb, CultureInfo.InvariantCulture);
			using (JsonTextWriter jsonWriter = new JsonTextWriter(sw))
			{
				jsonWriter.Formatting = jsonSerializer.Formatting;

				jsonSerializer.Serialize(jsonWriter, value, type);
			}

			return sw.ToString();
		}

		private static T DeserializeObject<T>(string value, JsonSerializer jsonSerializer)
			where T : class
		{
			ValidationUtils.ArgumentNotNull(value, nameof(value));

			// by default DeserializeObject should check for additional content
			if (!jsonSerializer.IsCheckAdditionalContentSet())
			{
				jsonSerializer.CheckAdditionalContent = true;
			}

			using (JsonTextReader reader = new JsonTextReader(new StringReader(value)))
			{
				return jsonSerializer.Deserialize(reader, typeof(T)) as T;
			}
		}

		public byte[] SerializeEntities(ECS ecs)
		{
			return SerializeEntities(ecs.EntityRegistry.Entities);
		}

		public byte[] SerializeEntities(EntityDictionary entityDictionary)
		{
			var serializedString = SerializeObjectInternal(entityDictionary, entityDictionary.GetType(), _entitySerializer);

			return Encoding.UTF8.GetBytes(serializedString);
		}

		public EntityDictionary DeserializeEntities(byte[] entityCollectionBytes)
		{
			var objString = Encoding.UTF8.GetString(entityCollectionBytes);
			
			return DeserializeObject<EntityDictionary>(objString, _entitySerializer);
		}

		#region compression

		public static byte[] Compress(byte[] input)
		{
			using (var inputStream = new MemoryStream(input))
			using (var outputStream = new MemoryStream())
			{
				GZip.Compress(inputStream, outputStream, false, 5);
				return outputStream.ToArray();
			}
		}

		public static byte[] Decompress(byte[] input)
		{
			using (var inputStream = new MemoryStream(input))
			using (var outputStream = new MemoryStream())
			{
				GZip.Decompress(inputStream, outputStream, false);
				return outputStream.ToArray();
			}
		}

		#endregion
	}
}
