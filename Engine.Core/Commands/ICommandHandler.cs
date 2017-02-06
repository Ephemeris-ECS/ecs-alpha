﻿using System;
using Engine.Systems;

namespace Engine.Commands
{
	public interface ICommandHandler : ISystemExtension
	{
		Type HandlesType { get; }

		bool TryProcessCommand(ICommand command);

		bool Enabled { get; }

		void SetEnabled(bool enabled);
	}
}