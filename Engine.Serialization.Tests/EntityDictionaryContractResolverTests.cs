using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Engine.Serialization.Tests
{
	[TestFixture()]
	public class DictionaryContractResolverTests
	{
		public class Outer
		{
			public Dictionary<int, Inner> Values { get; set; }

			public Outer()
			{
			}
		}

		public class Inner
		{
			public int Value { get; set;  }

			public Inner()
			{
			}
		}

		[Test]
		public void TestDefaultDictionaryResolver()
		{
			const string json = "{ Values: { 1: { Value: 1 }, 2: { Value: 2 } } }";

			var outer = JsonConvert.DeserializeObject<Outer>(json);

		}

	}
}
