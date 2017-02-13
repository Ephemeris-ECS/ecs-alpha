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
	public class EntityStateSerializer : SerializerBase
	{
		private readonly JsonSerializer _entitySerializer;

		#region constrcutors

		public EntityStateSerializer(DiContainer container, 
			IMatcherProvider matcherProvider)
		{
			var entitySerializerSettings = new EntityDictionarySerializerSettings();
			var entityContractResolver = new ECSContractResolver(container, matcherProvider)
			{
				TrackDeserializedEntities = entitySerializerSettings.PruneEntitiesOnDeserialize
			};
			entitySerializerSettings.ContractResolver = entityContractResolver;
			_entitySerializer = JsonSerializer.CreateDefault(entitySerializerSettings);

		}

		#endregion

		#region serialziation

		public string SerializeEntities(IECS ecs)
		{
			return SerializeEntities(ecs.Entities);
		}

		public string SerializeEntities(IECS ecs, out uint crc)
		{
			return SerializeEntities(ecs.Entities, out crc);
		}

		public string SerializeEntities(EntityDictionary entityDictionary)
		{
			uint crc;
			return SerializeEntities(entityDictionary, out crc);
		}

		public string SerializeEntities(EntityDictionary entityDictionary, out uint crc)
		{
			var entityStateJson = SerializeObjectInternal(entityDictionary, entityDictionary.GetType(), _entitySerializer);
			var entityStateBytes = Encoding.UTF8.GetBytes(entityStateJson);

			// TODO: understand how this works an decide if it needs custom polynomial
			// TODO: it has a stream mode where the previous result is used to seed the next, perhaps this is applicable here
			crc = Crc32.Compute(entityStateBytes);

			return entityStateJson;
		}

		#endregion

		#region deserialization

		public EntityDictionary DeserializeEntities(string entityCollectionJson)
		{
			return DeserializeObject<EntityDictionary>(entityCollectionJson, _entitySerializer);
		}

		public EntityDictionary DeserializeEntities(byte[] entityCollectionBytes)
		{
			var entityCollectionJson = Encoding.UTF8.GetString(entityCollectionBytes);

			return DeserializeEntities(entityCollectionJson);
		}

		#endregion
	}
}
