using Engine.Sequencing;
// ReSharper disable InconsistentNaming

namespace Engine.Configuration
{
	public abstract class Scenario<TECS, TECSConfiguration>
		where TECS : class, IECS
		where TECSConfiguration : ECSConfiguration
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public int MinPlayers { get; set; }

		public int MaxPlayers { get; set; }

		public TECSConfiguration Configuration { get; set; }

		public SequenceFrame<TECS>[] Sequence { get; set; }
	}
}
