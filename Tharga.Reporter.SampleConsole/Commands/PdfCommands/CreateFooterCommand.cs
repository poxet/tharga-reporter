using System;
using System.Threading.Tasks;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.SampleConsole.Commands.PdfCommands
{
    public class CreateFooterCommand : ActionCommandBase
    {
        public CreateFooterCommand()
            : base("footer", "create a simple document footer elements")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var section = new Section { Name = "Main", Margin = new UnitRectangle { Left = "2cm", Right = "1cm", Top = "1cm", Bottom = "1cm" } };

            section.Footer.Height = "3cm";
            section.Footer.ElementList.Add(new Line { Width = "100%", Height = "100%" });
            section.Footer.ElementList.Add(new Line { Top = "100%", Width = "100%", Height = "-100%" });
            section.Footer.ElementList.Add(new Text { Value = "Some text in the footer" + Environment.NewLine + "on two lines" });

            section.Footer.ElementList.Add(new TextBox { Left = "5cm", Value = "Some more text in the footer on two lines" });

            var template = new Template(section);

            await PdfCommand.RenderPdfAsync(template);

            return true;
        }
    }
}