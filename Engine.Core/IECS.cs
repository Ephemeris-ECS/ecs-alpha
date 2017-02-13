﻿using System.Security.Cryptography.X509Certificates;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Systems;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public interface IECS
	{
		EntityDictionary Entities { get; }

		bool TryGetSystem<TSystem>(out TSystem system) where TSystem : class, ISystem;

		int CurrentTick { get; }

		void Tick();

		IMatcherProvider MatcherProvider { get; }
	}

	// ReSharper disable once InconsistentNaming
	public interface IECS<out TConfiguration> : IECS
		where TConfiguration : ECSConfiguration
	{

		TConfiguration Configuration { get; }
	}
}