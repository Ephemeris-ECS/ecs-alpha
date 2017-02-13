using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Commands;

namespace Engine.Lifecycle
{
	public class Tick
	{
		public int CurrentTick { get; set; }

		public ICommand[] CommandQueue { get; set; }

		public uint Crc32 { get; set; }
	}
}
