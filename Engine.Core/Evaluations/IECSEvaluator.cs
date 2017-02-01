using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Evaluations
{
	public interface IECSEvaluator<TECS>
		where TECS : class, IECS
	{
		bool Evalulate(TECS ecs);
	}
}
