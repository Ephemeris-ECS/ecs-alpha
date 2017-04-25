using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Logging.Database.Model
{
	public class GameInstance
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public string ScenarioId { get; set; }

		public DateTime Initialized { get; set; }

		public virtual IList<Player> Players { get; set; }

		public virtual IList<Event> Events { get; set; }
	}
}
