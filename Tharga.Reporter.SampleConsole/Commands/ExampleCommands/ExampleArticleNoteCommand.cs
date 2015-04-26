using System.Drawing;
using System.Drawing.Printing;
using System.Threading.Tasks;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;
using Font = Tharga.Reporter.Engine.Entity.Font;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class ExampleArticleNoteCommand : ActionCommandBase
    {
        public ExampleArticleNoteCommand()
            : base("label", "Create an example artricle label.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var bgImage = @"C:\skalleberg_v1.png"; //_settingBusiness.GetSetting("BackgroundImageUrl");
            var image = new Image
            {
                Source = bgImage,
                Top = "48%",
                Height = "48%",
                IsBackground = true
            };

            var section = new Section { Margin = new UnitRectangle { Left = "6mm", Top = "2mm", Bottom = "2mm", Right = "6mm" } };
            section.Pane.ElementList.Add(new BarCode { Code = "ABC123", Top = "10%", Left = "20%", Width = "75%", Height = "60%" });
            section.Pane.ElementList.Add(image);
            section.Pane.ElementList.Add(new Text { Value = "Begonia", Font = new Font { Size = 18 } });
            section.Pane.ElementList.Add(new Text { Value = "100.00 Kr", Font = new Font { Size = 18 }, TextAlignment = TextBase.Alignment.Right });
            section.Pane.ElementList.Add(new Text { Value = "Holland", TextAlignment = TextBase.Alignment.Right, Left = "100%", Top = "90%" });
            var template = new Template(section);

            var documentProperties = new DocumentProperties
            {
            };

            var sampleData = new DocumentData();

            var pageSizeInfo = new PageSizeInfo("89mm", "36mm");

            //await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, pageSizeInfo, false, true);

            var renderer = new Renderer(template, sampleData, documentProperties, pageSizeInfo, false);
            var printerSettings = new PrinterSettings { };
            renderer.Print(printerSettings, true);

            return true;
        }
    }
}