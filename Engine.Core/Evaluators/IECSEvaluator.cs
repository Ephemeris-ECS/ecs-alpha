// ReSharper disable InconsistentNaming
namespace Engine.Evaluators
{
	public interface IECSEvaluator<in TECS> : IEvaluator
		where TECS : class, IECS
	{
		bool Evaluate(TECS ecs);

		void Activate();
	}
}
