using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Engine.Serialization
{
	// ReSharper disable once InconsistentNaming
	internal class ConfigurationSerializerSettings : JsonSerializerSettings
	{
		// TODO: I dont like the way this is constructed!
		public ConfigurationSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Auto;
			Converters.Add(new StringEnumConverter());
			
			MaxDepth = 256;

			// TODO: check if these work with the dictionary serialization changes!
			DefaultValueHandling = DefaultValueHandling.Include;
			NullValueHandling = NullValueHandling.Include;
			Formatting = Formatting.None;
			// TODO: I enabled these without testing the consequences! they are probably useful for the scenarios but apparently not for the configuration
			//PreserveReferencesHandling = PreserveReferencesHandling.All;
			//ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
		}
	}
}
