using Godot;
using System;
using System.Threading.Tasks;
using System.Text;

public class LocalLogger : ILogger
{
	private Enumerations.LogLevels LogLevel { get; set; }

	public LocalLogger(Enumerations.LogLevels logLevel)
	{
		LogLevel = logLevel;
	}

	public async Task Log(Enumerations.LogLevels logLevel, string message, Exception? exception = null)
	{
		if ((int)LogLevel <= (int)logLevel)
		{
			PublishLog(logLevel, message, exception);
		}
	}

	private async Task PublishLog(Enumerations.LogLevels logLevel, string message, Exception? exception = null)
	{
		try
		{
			var stringBuilder = new StringBuilder();
			stringBuilder.Append(message);
			if (exception != null && !string.IsNullOrEmpty(exception.Message)) stringBuilder.Append(" Exception: ").Append(exception.Message);
			if (exception?.InnerException != null && !string.IsNullOrEmpty(exception.InnerException.Message)) stringBuilder.Append(" Inner Exception").Append(exception.InnerException.Message);
			string str = stringBuilder.ToString();

			switch(logLevel)
			{
				case Enumerations.LogLevels.Error:
					GD.PrintErr(str);
					break;
				default:
					GD.Print(str);
					break;
			}
		}
		catch (Exception ex)
		{
			GD.PrintErr($"LocalLogger PublishLog Failed: {ex.Message} {ex.StackTrace}");
		}
	}
}
