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
	}
}
