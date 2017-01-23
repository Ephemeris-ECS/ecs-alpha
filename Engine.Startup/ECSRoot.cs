using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Configuration;
using Engine.Serialization;
// ReSharper disable InconsistentNaming

namespace Engine.Startup
{
	public class ECSRoot<TECS, TConfiguration>
		where TECS : ECS<TConfiguration>
		where TConfiguration : ECSConfiguration
	{
		public TECS ECS { get; }

		private TConfiguration Configuration { get; }

		public EntityStateSerializer EntityStateSerializer { get; }

		public ECSRoot(TECS ecs, TConfiguration configuration, EntityStateSerializer entityStateSerializer)
		{
			ECS = ecs;
			Configuration = configuration;
			EntityStateSerializer = entityStateSerializer;
		}

		/// <returns></returns>
		public string GetEntityState()
		{
			return EntityStateSerializer.SerializeEntities(ECS);
		}

		/// <summary>
		/// Merges the serialized representation of the entity dictionary into the current instance
		/// </summary>
		/// <param name="json"></param>
		public void UpdateEntityState(string json)
		{
			EntityStateSerializer.DeserializeEntities(json);
		}

		/// <summary>
		/// Gets a serialized representation of the 
		/// </summary>
		/// <returns></returns>
		public string GetConfiguration()
		{
			return ConfigurationSerializer.SerializeConfiguration(Configuration);
		}
	}
}

