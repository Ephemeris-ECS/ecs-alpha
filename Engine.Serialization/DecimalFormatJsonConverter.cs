using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Engine.Serialization
{
	public class DecimalFormatJsonConverter : JsonConverter
	{
		private const int NumberOfDecimals = 5;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var d = (decimal)value;
			writer.WriteValue(d.ToString($"F{NumberOfDecimals}"));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
			JsonSerializer serializer)
		{
			throw new NotImplementedException("Unnecessary because CanRead is false. The type will skip the converter.");
		}

		public override bool CanRead => false;

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(decimal);
		}
	}
}
