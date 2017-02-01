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


		public EntityStateSerializer(DiContainer container, 
			IComponentRegistry componentRegistry)
		{
			var entitySerializerSettings = new EntityDictionarySerializerSettings();
			var entityContractResolver = new ECSContractResolver(container, componentRegistry)
			{
				TrackDeserializedEntities = entitySerializerSettings.PruneEntitiesOnDeserialize
			};
			entitySerializerSettings.ContractResolver = entityContractResolver;
			_entitySerializer = JsonSerializer.CreateDefault(entitySerializerSettings);
		}

		public string SerializeEntities(IECS ecs)
		{
			return SerializeEntities(ecs.Entities);
		}

		public string SerializeEntities(EntityDictionary entityDictionary)
		{
			return SerializeObjectInternal(entityDictionary, entityDictionary.GetType(), _entitySerializer, false);
		}

		public EntityDictionary DeserializeEntities(string entityCollectionJson)
		{
			return DeserializeObject<EntityDictionary>(entityCollectionJson, _entitySerializer);
		}

		public EntityDictionary DeserializeEntities(byte[] entityCollectionBytes)
		{
			var entityCollectionJson = Encoding.UTF8.GetString(entityCollectionBytes);

			return DeserializeEntities(entityCollectionJson);
		}
	}
}
