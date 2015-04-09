using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.SampleConsole.Commands.ExampleCommands
{
    public class InvoiceArticleNoteCommand : ActionCommandBase
    {
        public InvoiceArticleNoteCommand()
            : base("label", "Create an example artricle label.")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            //TODO: Create a lable here

            return true;
        }
    }
}