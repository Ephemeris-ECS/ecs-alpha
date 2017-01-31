using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Engine.Serialization
{
	// ReSharper disable once InconsistentNaming
	internal class EntityDictionarySerializerSettings : JsonSerializerSettings
	{
		public bool PruneEntitiesOnDeserialize { get; set; } = true;

		// TODO: I dont like the way this is constructed!
		public EntityDictionarySerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto;
			TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple;
			Converters.Add(new StringEnumConverter());
			
			// this shouldn't need to be very deep now that we have just the lightweight entites being copied
			MaxDepth = 64;

			// TODO: check if these work with the dictionary serialization changes!
			DefaultValueHandling = DefaultValueHandling.Include;
			NullValueHandling = NullValueHandling.Ignore;
			Formatting = Formatting.None;
		}
	}
}
