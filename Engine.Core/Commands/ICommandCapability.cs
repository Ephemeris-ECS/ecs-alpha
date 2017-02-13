using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Engine.Commands
{
	public interface ICommandCapability
	{
		// TODO: I dont like this pattern on either the capability extensions 
		// it would be nicer to be able to inject these into the command handler extension configuration
		Type HandlesType { get; }

		bool Evalutate();

	}
}
