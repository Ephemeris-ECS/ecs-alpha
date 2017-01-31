﻿using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public interface IECS
	{
		EntityDictionary Entities { get; }

		Entity CreateEntityFromArchetype(string archetypeName);

		bool TryGetSystem<TSystem>(out TSystem system) where TSystem : class, ISystem;
	}

	// ReSharper disable once InconsistentNaming
	public interface IECS<out TConfiguration> : IECS
		where TConfiguration : ECSConfiguration
	{

		TConfiguration Configuration { get; }
	}
}