using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Systems;
using Zenject;

namespace Engine.Commands
{
	public class CommandSystem : ISystem, ICommandSystem
	{
		private readonly Dictionary<Type, ICommandHandler> _commandHandlers;

		private readonly Dictionary<Type, ICommandCapability[]> _commandCapabilities;

		public CommandSystem([InjectOptional] List<ICommandHandler> commandHandlers,
			[InjectOptional] List<ICommandCapability> commandCapabilities) 
		{
			_commandHandlers = commandHandlers.ToDictionary(k => k.HandlesType, v => v);
			_commandCapabilities = commandCapabilities.GroupBy(cc => cc.HandlesType).ToDictionary(k => k.Key, v => v.ToArray());
		}

		/// <summary>
		/// Attempt to handle a command
		/// </summary>
		/// <param name="command">Command object</param>
		/// <returns>true if the command capability evaluators are successful and there is a valida handler registered and the command is successfully applied by the handler</returns>
		public bool TryHandleCommand(ICommand command)
		{
			ICommandHandler commandHandler;
			if (TryGetCapabiliity(command.GetType(), out commandHandler))
			{
				return commandHandler.Enabled && commandHandler.TryProcessCommand(command);
			}
			return false;
		}

		public bool TryGetHandler(Type commandType, out ICommandHandler commandHandler)
		{
			return _commandHandlers.TryGetValue(commandType, out commandHandler);
		}

		public bool TryGetHandler<TCommand>(out ICommandHandler commandHandler)
			where TCommand : ICommand
		{
			return TryGetHandler(typeof(TCommand), out commandHandler);
		}

		private bool TryGetCapabiliity(Type commandType, out ICommandHandler commandHandler)
		{
			return TryGetHandler(commandType, out commandHandler)
				&& (_commandCapabilities.TryGetValue(commandType, out var capabilities) == false || capabilities.All(cc => cc.Evalutate()));

		}

		/// <summary>
		/// Determine whether the command capability exists
		/// </summary>
		/// <typeparam name="TCommand">Command object</typeparam>
		/// <returns>true if there is a handler registered and all of the capability evaluations are successful</returns>
		public bool TryGetCapabiliity<TCommand>()
			where TCommand : ICommand
		{
			ICommandHandler commandHandler;
			return TryGetCapabiliity(typeof(TCommand), out commandHandler);

		}
	}
}
