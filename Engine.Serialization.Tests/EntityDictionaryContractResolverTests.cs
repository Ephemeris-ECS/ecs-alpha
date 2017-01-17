using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Engine.Newtonsoft.Json.Tests
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
