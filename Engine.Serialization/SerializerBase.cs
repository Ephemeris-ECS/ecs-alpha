using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

namespace Engine.Serialization
{
	public abstract class SerializerBase
	{
		protected static string SerializeObjectInternal<T>(T value, Type type, JsonSerializer jsonSerializer, bool quoteNames = true)
		{
			var sb = new StringBuilder(256);
			var sw = new StringWriter(sb, CultureInfo.InvariantCulture);
			using (var jsonWriter = new JsonTextWriter(sw))
			{
				jsonWriter.QuoteName = quoteNames;
				jsonWriter.Formatting = jsonSerializer.Formatting;

				jsonSerializer.Serialize(jsonWriter, value, type);
			}

			return sw.ToString();
		}

		protected static T DeserializeObject<T>(string value, JsonSerializer jsonSerializer)
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

	}
}
