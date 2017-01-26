using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Testing
{
	public class TestComponentA : IComponent
	{
		public string StringValuePublic { get; set; }

		private string _stringValuePrivate;

		public int IntValuePublic { get; set; }

		private int _intValuePrivate;
	}
}
