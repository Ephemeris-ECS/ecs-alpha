using System;

namespace Engine.Commands
{
	public interface ICommand : IEquatable<ICommand>
	{
		// TODO: implement this via some binding to the handler and and evaluation
		// bool CanExecute { get; }
	}
}
