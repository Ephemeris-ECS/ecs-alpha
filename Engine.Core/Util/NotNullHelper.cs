﻿using System;

namespace Engine.Util
{
	public static class NotNullHelper
	{
		public static void ArgumentNotNull(object obj, string membername)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(membername);
			}
		}
	}
}
