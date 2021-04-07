using System;
using GoCommando;
using ColorConsole;
using MCE2E.Controller.Bootstrapping;

namespace MCE2E.Cli.Commands
{
	public abstract class BaseCommand : ICommand
	{
		private readonly ConsoleWriter _consoleWriter;
		protected IServiceProvider ServiceProvider;

		protected BaseCommand()
		{
			_consoleWriter = new ConsoleWriter();
			_consoleWriter.SetForeGroundColor(ConsoleColor.White);
		}

		protected void Initialize()
		{
			ServiceProvider = new EncryptionServiceProviderBuilder().Build();
		}

		public abstract void Run();

		[Parameter("plugin_dir", optional: true, defaultValue: "")]
		[Description("The absolute path to the directory containing plugins.")]
		public string PluginDirectory { get; set; }

		protected void Log(string message, LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Info: 
					_consoleWriter.SetForeGroundColor(ConsoleColor.White);
					break;
				case LogLevel.Warn:
					_consoleWriter.SetForeGroundColor(ConsoleColor.DarkYellow);
					break;
				case LogLevel.Error:
					_consoleWriter.SetForeGroundColor(ConsoleColor.Red);
					break;
			}

			_consoleWriter.WriteLine($"{DateTime.Now.ToString("hh:mm:ss tt")}:{message}");
		}

		protected void Log(Exception ex)
		{
			Log(ex.Message, LogLevel.Error);
			if (ex.InnerException != null)
			{
				Log(ex.InnerException.Message, LogLevel.Error);
			}
		}
	}

	public enum LogLevel
	{
		Info,
		Warn,
		Error
	}
}
