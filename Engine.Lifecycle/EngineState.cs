using System;
using System.Collections.Generic;
using System.Text;

namespace Engine.Lifecycle
{
	public enum EngineState
	{
		NotStarted = 0,
		Started,
		Paused,
		Stopped,

		Error,
	}
}
