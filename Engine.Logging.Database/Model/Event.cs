using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Logging.Database.Model
{
	public class Event
	{
		public Guid GameId { get; set; }
		public virtual GameInstance Game { get; set; }
		
		public int? PlayerId { get; set; }
			
		public virtual Player Player { get; set; }

		public string Data { get; set; }

		public int EventId { get; set; }

		public string EventCode { get; set; }

		public int Tick { get; set; }
	}
}
