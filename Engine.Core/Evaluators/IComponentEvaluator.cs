using Engine.Components;

namespace Engine.Evaluators
{
	public interface IComponentEvaluator
	{
		bool Evaluate(IComponent component);
	}
}
