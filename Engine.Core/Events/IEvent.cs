namespace Engine.Events
{
	public interface IEvent
	{
		int Tick { get; }

		int Sequence { get; }

		void SetCounters(int tick, int sequence);
	}
}
