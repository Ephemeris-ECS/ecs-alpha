using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Entities;

namespace Engine.Evaluations
{
	public interface IEntityEvaluator
	{
		bool Evalulate(Entity entity);
	}
}
