// ReSharper disable InconsistentNaming

using System;
using Engine.Configuration;

namespace Engine.Evaluators
{
	public interface IEvaluator : IDisposable
	{
		
	}

	public interface IEvaluator<in TECS, in TConfiguration> : IEvaluator
		where TECS : class, IECS
		where TConfiguration : ECSConfiguration
	{
		void Initialize(TECS ecs, TConfiguration configuration);
		bool Evaluate(TECS ecs, TConfiguration configuration);
	}
}
