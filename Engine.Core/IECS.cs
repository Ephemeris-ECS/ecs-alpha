using System.Security.Cryptography.X509Certificates;
using Engine.Commands;
using Engine.Components;
using Engine.Configuration;
using Engine.Entities;
using Engine.Lifecycle;
using Engine.Systems;

namespace Engine
{
	// ReSharper disable once InconsistentNaming
	public interface IECS
	{
		EntityDictionary Entities { get; }

		bool TryGetSystem<TSystem>(out TSystem system) where TSystem : class, ISystem;

		int CurrentTick { get; }

		Tick Tick();

		IMatcherProvider MatcherProvider { get; }

		void EnqueueCommand(ICommand command);
	}

	// ReSharper disable once InconsistentNaming
	public interface IECS<out TConfiguration> : IECS
		where TConfiguration : ECSConfiguration
	{

		TConfiguration Configuration { get; }
	}
}