using System.Threading.Tasks;
using Tharga.Reporter.Engine.Entity;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.SampleConsole.Commands.PdfCommands
{
    public class CreateBlankCommand : ActionCommandBase
    {
        public CreateBlankCommand()
            : base("blank", "create a blank document")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var section = new Section { Name = "Main", Margin = new UnitRectangle { Left = "2cm", Right = "1cm", Top = "3cm", Bottom = "3cm" } };
            var template = new Template(section);

            await PdfCommand.RenderPdfAsync(template);

            return true;
        }
    }
}