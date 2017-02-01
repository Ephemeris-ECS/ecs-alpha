using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Components;

namespace Engine.Evaluations
{
	public interface IComponentEvaluator
	{
		bool Evaluate(IComponent component);
	}
}
