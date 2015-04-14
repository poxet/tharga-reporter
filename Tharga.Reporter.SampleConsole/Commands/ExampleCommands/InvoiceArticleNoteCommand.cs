using System.Threading.Tasks;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class InvoiceArticleNoteCommand : ActionCommandBase
    {
        public InvoiceArticleNoteCommand()
            : base("label", "Create an example artricle label.")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            var section = new Section { };
            section.Pane.ElementList.Add(new Text { Value = "My label" });
            var template = new Template(section);

            var documentProperties = new DocumentProperties
            {
            };

            var sampleData = new DocumentData();

            var pageSizeInfo = new PageSizeInfo("8cm","4cm");

            await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, pageSizeInfo, false);

            return true;
        }
    }
}