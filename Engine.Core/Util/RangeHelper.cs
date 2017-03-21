using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Util
{
	public static class RangeHelper
	{
		public static int AssignWithinBounds(int value, int min, int max)
		{
			return Math.Max(min, Math.Min(value, max));
		}

		public static decimal AssignWithinBounds(decimal value, decimal min, decimal max)
		{
			return Math.Max(min, Math.Min(value, max));
		}
	}
}
