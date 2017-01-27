using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Util
{
	public static class DefaultValueHelper
	{
		public static object GetDefault(Type type)
		{
			if (type.IsValueType)
			{
				return Activator.CreateInstance(type);
			}
			return null;
		}
	}
}
