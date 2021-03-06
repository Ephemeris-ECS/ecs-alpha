﻿using System;
using System.Collections.Generic;
using Engine.Systems;

namespace Engine.Commands
{
	public interface ICommandHandler : ISystemExtension
	{
		Type HandlesType { get; }

		bool TryHandleCommand(ICommand command, int currentTick);

		bool Enabled { get; }

		void SetEnabled(bool enabled);

		IEqualityComparer<ICommand> Deduplicator { get; }
	}
}
