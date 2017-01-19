using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public interface IECS
	{
		Entity CreateEntityFromArchetype(string archetypeName);

		IEntityRegistry EntityRegistry { get; }
		IComponentRegistry ComponentRegistry { get; }

		ISystemRegistry SystemRegistry { get; }
	}

	// ReSharper disable once InconsistentNaming
	public interface IECS<out TConfiguration> : IECS
		where TConfiguration : ECSConfiguration
	{

		TConfiguration Configuration { get; }
	}
}