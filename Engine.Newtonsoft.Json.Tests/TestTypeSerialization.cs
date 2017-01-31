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
	public class TestTypeSerialization
	{
		public class TestClass
		{
			public Type Type { get; set; }
		}

		[Test]
		public void TestSerializeType()
		{
			var a = new TestClass()
			{
				Type = typeof(TestClass)
			};

			var json = JsonConvert.SerializeObject(a);
		}

	}
}
