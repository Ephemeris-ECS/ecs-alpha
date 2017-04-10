using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Common.Logging;

namespace Engine.Logging
{
	public class DummyLogger : ILogger
	{
		public Type Type { get; }
		public string Name { get; }
		public bool IsDebugEnabled { get; }
		public bool IsInfoEnabled { get; }
		public bool IsTraceEnabled { get; }
		public bool IsWarnEnabled { get; }
		public bool IsErrorEnabled { get; }
		public bool IsFatalEnabled { get; }

		public void Debug(string message)
		{
			// do nothing
		}

		public void Debug(string format, params object[] args)
		{
			// do nothing
		}

		public void Debug(Exception exception, string format, params object[] args)
		{
			// do nothing
		}

		public void DebugException(string message, Exception exception)
		{
			// do nothing
		}

		public void Info(string message)
		{
			// do nothing
		}

		public void Info(string format, params object[] args)
		{
			// do nothing
		}

		public void Info(Exception exception, string format, params object[] args)
		{
			// do nothing
		}

		public void InfoException(string message, Exception exception)
		{
			// do nothing
		}

		public void Trace(string message)
		{
			// do nothing
		}

		public void Trace(string format, params object[] args)
		{
			// do nothing
		}

		public void Trace(Exception exception, string format, params object[] args)
		{
			// do nothing
		}

		public void TraceException(string message, Exception exception)
		{
			// do nothing
		}

		public void Warn(string message)
		{
			// do nothing
		}

		public void Warn(string format, params object[] args)
		{
			// do nothing
		}

		public void Warn(Exception exception, string format, params object[] args)
		{
			// do nothing
		}

		public void WarnException(string message, Exception exception)
		{
			// do nothing
		}

		public void Error(string message)
		{
			// do nothing
		}

		public void Error(string format, params object[] args)
		{
			// do nothing
		}

		public void Error(Exception exception, string format, params object[] args)
		{
			// do nothing
		}

		public void ErrorException(string message, Exception exception)
		{
			// do nothing
		}

		public void Fatal(string message)
		{
			// do nothing
		}

		public void Fatal(string format, params object[] args)
		{
			// do nothing
		}

		public void Fatal(Exception exception, string format, params object[] args)
		{
			// do nothing
		}

		public void FatalException(string message, Exception exception)
		{
			// do nothing
		}
	}
}
