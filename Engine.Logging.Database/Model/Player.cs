using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Logging.Database.Model
{
	public class Player
	{
		public Guid GameId { get; set; }
		public virtual GameInstance Game { get; set; }

		public int Id { get; set; }
		
		public virtual IList<Event> Events { get; set; }

		public string PlayerIdentifier { get; set; }

	}
}
