using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.SampleConsole.Commands.ExampleCommands
{
    public class ExampleCommands : ContainerCommandBase
    {
        public ExampleCommands() 
            : base("example")
        {
            RegisterCommand(new InvoiceExampleCommand());
            RegisterCommand(new InvoiceArticleNoteCommand());
        }
    }
}