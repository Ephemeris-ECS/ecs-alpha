namespace Engine.Entities
{
	public interface IEntityFactoryProvider
	{
		bool TryCreateEntityFromArchetype(string archetypeName, out Entity entity);
	}
}