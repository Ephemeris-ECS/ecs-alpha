﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine
{
	public interface ITickable
	{
		void Tick(int currentTick);
	}
}
