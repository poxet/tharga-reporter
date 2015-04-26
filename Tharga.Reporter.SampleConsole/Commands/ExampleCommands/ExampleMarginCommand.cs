using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class ExampleMarginCommand : ActionCommandBase
    {
        public ExampleMarginCommand() 
            : base("Margin", "Create a margin example.")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            var section = new Section(); // { Margin = new UnitRectangle { Left = "1cm", Top = "1cm", Bottom = "1cm", Right = "1cm" } };
            section.Pane.ElementList.Add(new Rectangle { BorderColor = Color.Black, Left = "1cm", Top = "1cm", Bottom = "1cm", Right = "1cm" });
            section.Pane.ElementList.Add(new Line { Top = "0", Left = "0", Bottom = "0", Right = "0" });
            var template = new Template(section);

            var documentProperties = new DocumentProperties
            {
            };

            var sampleData = new DocumentData();

            var pageSizeInfo = new PageSizeInfo("A4");

            await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, pageSizeInfo, false, true);

            //var renderer = new Renderer(template, sampleData, documentProperties, pageSizeInfo, false);
            //var printerSettings = new PrinterSettings { };
            //renderer.Print(printerSettings, true, true);

            return true;
        }
    }
}