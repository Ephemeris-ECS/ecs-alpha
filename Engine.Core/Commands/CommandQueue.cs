using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Engine.Util;

namespace Engine.Commands
{
	public class CommandQueue
	{
		private readonly MutableQueue<ICommand> _commandQueue;

		private readonly object _commandQueueLock = new object();

		private readonly ICommandSystem _commandSystem;

		private readonly Dictionary<Type, DeduplicationPolicy> _deduplicationPolicies;

		private readonly Dictionary<Type, IEqualityComparer<ICommand>> _deduplicationComparers;

		public CommandQueue(ICommandSystem commandSystem)
		{
			_commandQueue = new MutableQueue<ICommand>(100);
			_commandSystem = commandSystem;
			_deduplicationPolicies = new Dictionary<Type, DeduplicationPolicy>();
			_deduplicationComparers = new Dictionary<Type, IEqualityComparer<ICommand>>();
		}

		public void EnqueueCommand(ICommand command)
		{
			lock (_commandQueueLock)
			{
				Deduplicate(command);
			}
		}

		private void Deduplicate(ICommand command)
		{
			var commandType = command.GetType();
			if (_deduplicationPolicies.TryGetValue(commandType, out var deduplicationPolicy) == false)
			{
				deduplicationPolicy = AttributeHelper.SelectValues<DeduplicateAttribute, DeduplicationPolicy>(commandType, a => a.DeduplicationPolicy).SingleOrDefault();
				_deduplicationPolicies.Add(commandType, deduplicationPolicy);
			}

			switch (deduplicationPolicy)
			{
				case DeduplicationPolicy.Discard:
				case DeduplicationPolicy.Replace:
					Deduplicate(command, deduplicationPolicy);
					break;
			}
		}

		private void Deduplicate(ICommand command, DeduplicationPolicy policy)
		{
			var commandType = command.GetType();
			IEqualityComparer<ICommand> comparer;
			if (_deduplicationComparers.TryGetValue(commandType, out comparer) == false
				&& _commandSystem.TryGetHandler(commandType, out var commandHandler))
			{
				comparer = commandHandler.Deduplicator;
				_deduplicationComparers.Add(commandType, comparer);
			}

			lock (_commandQueueLock)
			{
				ICommand match = null;
				if (_commandQueue.TryGetItem(command, ref match, comparer))
				{
					switch (policy)
					{
						case DeduplicationPolicy.Discard:
							break;
						case DeduplicationPolicy.Replace:
							match = command;
							break;
						default:
							_commandQueue.Enqueue(command);
							break;
					}
				}
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
