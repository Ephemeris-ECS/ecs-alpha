using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Logging.Database.Model
{
	public class EventData
	{
		public Guid GameId { get; set; }

		public int EventId { get; set; }

		public virtual Event Event { get; set; }

		public string Key { get; set; }

		public string Value { get; set; }
	}
}
