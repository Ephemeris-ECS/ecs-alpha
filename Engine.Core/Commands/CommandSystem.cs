using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Systems;
using Zenject;

namespace Engine.Commands
{
	public class CommandSystem : ISystem
	{
		private readonly Dictionary<Type, ICommandHandler> _commandHandlers;

		public CommandSystem([InjectOptional] List<ICommandHandler> commandHandlers) 
		{
			_commandHandlers = commandHandlers.ToDictionary(k => k.HandlesType, v => v);
		}

		public bool TryHandleCommand(ICommand command)
		{
			ICommandHandler commandHandler;
			if (_commandHandlers.TryGetValue(command.GetType(), out commandHandler))
			{
				return commandHandler.TryProcessCommand(command);
			}
			return false;
		}
	}
}
