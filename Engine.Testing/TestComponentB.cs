using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Testing
{
	public class TestComponentB : IComponent
	{
		public Dictionary<int, int> DictionaryValuePublic { get; set; }

		private Dictionary<int, int> _dictionaryValuePrivate;
	}
}
