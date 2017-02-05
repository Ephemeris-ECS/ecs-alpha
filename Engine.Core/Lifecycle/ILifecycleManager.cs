using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Lifecycle
{
	public interface ILifecycleManager
	{
		bool TryStop();

		bool TryStart();

		bool TryPause();

		bool TryTick();
	}
}
