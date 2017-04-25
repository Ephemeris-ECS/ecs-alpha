using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Engine.Common.Logging;
using Engine.Util;
using Zenject;

namespace Engine.Commands
{
	public class CommandQueue
	{
		[Inject]
		public ILogger Logger { get; set; }

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

		/// <summary>
		/// Enqueue a command for processing
		/// </summary>
		/// <param name="command">Command to enqueue</param>
		/// <param name="preempt">Defaults to false. Skip deduplication and enqueue at the head of the queue. This is only intended for internal use.</param>
		public void EnqueueCommand(ICommand command, bool preempt = false)
		{
			lock (_commandQueueLock)
			{
				if (preempt)
				{
					_commandQueue.Enqueue(command, true);
					Logger.Debug($"Command enqueued with preempt: {command.GetType()}");
				}
				else
				{
					Deduplicate(command);
				}
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

			Deduplicate(command, deduplicationPolicy);
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
				switch (policy)
				{
					case DeduplicationPolicy.Discard:
						if (_commandQueue.TryGetItem(command, out var match, comparer))
						{
							Logger.Debug($"Command discarded due to deduplication policy: {command.GetType()}");
						}
						else
						{
							_commandQueue.Enqueue(command);
							Logger.Debug($"Command enqueued: {command.GetType()}");
						}
						break;
					case DeduplicationPolicy.Replace:
						if (_commandQueue.TryReplaceItem(command, comparer))
						{
							Logger.Debug($"Command replaced existing duplicate: {command.GetType()}");
						}
						else
						{
							_commandQueue.Enqueue(command);
							Logger.Debug($"Command enqueued: {command.GetType()}");
						}
						break;
					default:
						_commandQueue.Enqueue(command);
						Logger.Debug($"Command enqueued: {command.GetType()}");
						break;
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

		public ICommand[] Flush(int currentTick)
		{
			return DequeueCommands().Where(command => _commandSystem.TryHandleCommand(command, currentTick)).ToArray();
		}

	}
}
