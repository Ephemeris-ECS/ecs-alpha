using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable InconsistentNaming

namespace Engine.Sequencing
{
	public class ECSAction<TECS>
		where TECS : class, IECS
	{
		public Action<TECS> Action { get; set; }

		public string Name { get; set; }
	}
}
