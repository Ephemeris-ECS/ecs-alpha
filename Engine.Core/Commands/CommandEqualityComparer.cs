using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Commands
{
	public abstract class CommandEqualityComparer : EqualityComparer<ICommand>
	{
		
	}

	public abstract class CommandEqualityComparer<TCommand> : CommandEqualityComparer
		where TCommand : class, ICommand
	{
		protected abstract bool IsDuplicate(TCommand x, TCommand other);


		#region Overrides of EqualityComparer<ICommand>

		public override bool Equals(ICommand x, ICommand y)
		{
			return IsDuplicate(x as TCommand, y as TCommand);
		}

		#endregion

		public override int GetHashCode(ICommand obj)
		{
			// TODO: these probably arent in a dictionary
			return obj?.GetHashCode() ?? 0;
		}

	}
}
