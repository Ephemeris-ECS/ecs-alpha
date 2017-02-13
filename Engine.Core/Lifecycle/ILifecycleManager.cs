using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;

namespace Engine.Lifecycle
{
	public interface ILifecycleManager
	{
		bool TryStop();

		bool TryStart();

		bool TryPause();

		void EnqueueCommand(ICommand command);
	}
}
