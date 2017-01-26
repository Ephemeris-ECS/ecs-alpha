using Engine.Archetypes;

namespace Engine.Entities
{
	public interface IEntityFactory
	{
		Archetype Archetype { get; }

		Entity CreateEntity();
	}
}