using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Entities
{
	public sealed class EntityDictionary : Dictionary<int, Entity>
	{
		/// <summary>
		/// Entity dictionary
		/// </summary>
		public EntityDictionary() 
			// totally arbitrary default size TODO: work out a better way of guessing a default
			: base(1024)
		{
		}

		public new void Add(int key, Entity value)
		{
			if (key != value.Id)
			{
				throw new InvalidOperationException("Key does does not match entity Id."); // This isn't your average dictionary ;)
			}
			base.Add(key, value);
		}
	}
}
