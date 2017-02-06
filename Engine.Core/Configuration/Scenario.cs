using Engine.Sequencing;
// ReSharper disable InconsistentNaming

namespace Engine.Configuration
{
	public abstract class Scenario<TECS, TConfiguration>
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public int MinPlayers { get; set; }

		public int MaxPlayers { get; set; }

		public TConfiguration Configuration { get; set; }

		public SequenceFrame<TECS, TConfiguration>[] Sequence { get; set; }
	}
}
