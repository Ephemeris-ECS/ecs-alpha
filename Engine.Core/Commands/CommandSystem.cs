using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Commands;
using Engine.Components;
using Engine.Entities;
using Zenject;

namespace Engine.Systems
{
	public class CommandSystem : System
	{
		private readonly Dictionary<Type, ICommandHandler> _commandHandlers;

		public CommandSystem(IMatcherProvider matcherProvider,
			IEntityRegistry entityRegistry,
			[InjectOptional] List<ICommandHandler> commandHandlers) 
			: base (matcherProvider, entityRegistry)
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
