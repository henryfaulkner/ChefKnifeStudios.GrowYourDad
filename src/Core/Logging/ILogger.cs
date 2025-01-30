using System;
using System.Threading.Tasks;

public interface ILogger
{
	public Task Log(Enumerations.LogLevels logLevel, string message, Exception? exception);
}
