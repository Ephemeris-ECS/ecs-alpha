using System;

namespace Engine.Commands
{
	public class HaltAndCatchFireCommand : ICommand
	{
		#region Equality members

		protected bool Equals(HaltAndCatchFireCommand other)
		{
			return Equals((object) other);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return true;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		#region Implementation of IEquatable<ICommand>

		public bool Equals(ICommand other)
		{
			return Equals((object)other);
		}

		#endregion

		#endregion
	}

	public class HaltAndCatchFireCommandHandler : CommandHandler<HaltAndCatchFireCommand>
	{
		protected override bool TryProcessCommand(HaltAndCatchFireCommand command, int currentTick)
		{
			throw new HaltAndCatchFireException();
		}
	}

	public class HaltAndCatchFireException : Exception
	{
	}
}
