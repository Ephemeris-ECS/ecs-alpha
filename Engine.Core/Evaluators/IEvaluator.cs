// ReSharper disable InconsistentNaming

using Engine.Configuration;

namespace Engine.Evaluators
{
	public interface IEvaluator
	{
		
	}

	public interface IEvaluator<in TECS, in TConfiguration> : IEvaluator
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		bool Evaluate(TECS ecs, TConfiguration configuration);

	}
}
