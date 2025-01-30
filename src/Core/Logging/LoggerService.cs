using Godot;
using System;
using System.Collections.Generic;

public partial class LoggerService : Node, ILoggerService
{
	private List<ILogger> Loggers { get; set; }

	public LoggerService()
	{
		Loggers = new List<ILogger>();
		Loggers.Add(new LocalLogger(Enumerations.LogLevels.Info));
	}

	public void LogDebug(string message, Exception? exception = null)
	{
		Loggers.ForEach((logger) =>
			{
				logger.Log(Enumerations.LogLevels.Debug, message, exception);
			}
		);
	}

	public void LogInfo(string message, Exception? exception = null)
	{
		Loggers.ForEach((logger) =>
			{
				logger.Log(Enumerations.LogLevels.Info, message, exception);
			}
		);
	}

	public void LogError(string message, Exception? exception = null)
	{
		Loggers.ForEach((logger) =>
			{
				logger.Log(Enumerations.LogLevels.Error, message, exception);
			}
		);
	}
}
