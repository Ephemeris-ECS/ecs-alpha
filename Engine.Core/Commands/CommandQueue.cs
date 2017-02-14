using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Commands
{
	public class CommandQueue
	{
		private readonly Queue<ICommand> _commandQueue;

		private readonly object _commandQueueLock = new object();

		private readonly ICommandSystem _commandSystem;

		public CommandQueue(ICommandSystem commandSystem)
		{
			_commandQueue = new Queue<ICommand>(100);
			_commandSystem = commandSystem;
		}

		public void EnqueueCommand(ICommand command)
		{
			lock (_commandQueueLock)
			{
				_commandQueue.Enqueue(command);
			}
		}

		private ICommand[] DequeueCommands()
		{
			lock (_commandQueueLock)
			{
				var commands = new ICommand[_commandQueue.Count];
				_commandQueue.CopyTo(commands, 0);
				_commandQueue.Clear();
				return commands;
			}
		}

		public ICommand[] Flush()
		{
			return DequeueCommands().Where(command => _commandSystem.TryHandleCommand(command)).ToArray();
		}

	}
}
