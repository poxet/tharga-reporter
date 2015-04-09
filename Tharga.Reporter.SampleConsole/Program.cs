using Tharga.Reporter.SampleConsole.Commands.ExampleCommands;
using Tharga.Reporter.SampleConsole.Commands.PdfCommands;
using Tharga.Toolkit.Console;
using Tharga.Toolkit.Console.Command;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.SampleConsole
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var rootCommand = new RootCommand(new ClientConsole());
            rootCommand.RegisterCommand(new ExampleCommands());
            rootCommand.RegisterCommand(new PdfCommand());
            var engine = new CommandEngine(rootCommand);
            engine.Run(args);
        }
    }
}
