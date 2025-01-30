using System;

public interface ILoggerService
{
	public void LogDebug(string message, Exception? exception = null);
	public void LogInfo(string message, Exception? exception = null);
	public void LogError(string message, Exception? exception = null);
}
