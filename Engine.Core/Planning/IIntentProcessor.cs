using System;

namespace Engine.Planning
{
	public interface IIntentProcessor
	{
		Type HandlesIntent { get; }

		bool TryProcessIntent(IIntent intent);
	}
}
