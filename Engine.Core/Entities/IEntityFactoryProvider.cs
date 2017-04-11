namespace Engine.Entities
{
	public interface IEntityFactoryProvider
	{
		void InitializeFactories();

		bool TryCreateEntityFromArchetype(string archetypeName, out Entity entity);
	}
}