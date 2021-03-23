using GoCommando;
using System;
using System.IO;
using System.Linq;
using ColorConsole;

namespace MCE2E.Cli.Commands
{
    public abstract class BaseCommand : ICommand
    {
        private ConsoleWriter _consoleWriter;

        public BaseCommand()
        {
            _consoleWriter = new ConsoleWriter();
            _consoleWriter.SetForeGroundColor(ConsoleColor.White);
        }

        public abstract string TargetDirectory { get; set; }

        public abstract void Run();

        [Parameter("plugin_dir", optional: true, defaultValue: "")]
        [Description("The absolute path to the directory containing plugins.")]
        public string PluginDirectory { get; set; }

        protected bool ValidateArguments()
        {
            if (string.IsNullOrEmpty(TargetDirectory))
            {
                Log($"No value for {nameof(TargetDirectory)}", isError: true);
                return false;
            }
            if (!Directory.Exists(TargetDirectory))
            {
                Log($"{nameof(TargetDirectory)} does not exist", isError: true);
                return false;
            }

            if (!Directory.GetFiles(TargetDirectory).Any())
            {
                Log($"{nameof(TargetDirectory)} is empty", isError: true);
                return false;
            }

            return true;
        }

        protected void Log(string message, bool isError = false)
        {
            if (!isError)
            {
                _consoleWriter.WriteLine(message);
                return;
            }

            _consoleWriter.SetForeGroundColor(ConsoleColor.Red);
            _consoleWriter.WriteLine(message);
            _consoleWriter.SetForeGroundColor(ConsoleColor.White);
        }
    }
}
