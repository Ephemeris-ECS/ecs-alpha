using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Common.Logging
{
	public interface ILogSource
	{
		ILogger Logger { set; }
	}
}
