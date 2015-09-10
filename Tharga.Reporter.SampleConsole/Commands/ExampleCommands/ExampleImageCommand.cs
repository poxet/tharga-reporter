using System.Drawing;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;
using Image = Tharga.Reporter.Engine.Entity.Element.Image;
using Rectangle = Tharga.Reporter.Engine.Entity.Element.Rectangle;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class ExampleImageCommand : ActionCommandBase
    {
        public ExampleImageCommand() 
            : base("image", "Sample that generates a image from URL or byte[].")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            var section = new Section();
            section.Pane.ElementList.Add(new Rectangle { BorderColor = Color.Black, Left = "1cm", Top = "1cm", Bottom = "1cm", Right = "1cm" });
            section.Pane.ElementList.Add(new Line { Top = "0", Left = "0", Bottom = "0", Right = "0" });
            section.Pane.ElementList.Add(new Image { Source = "{Img1}", Height = "150", Top = "50", Left="50" });
            section.Pane.ElementList.Add(new Image { Source = "{Img2}", Height = "150", Top = "200", Left = "50" });
            var template = new Template(section);

            var documentProperties = new DocumentProperties
            {
            };

            //Image from url
            var sampleData = new DocumentData();
            sampleData.Add("Img1", "http://www.thargelion.se/Images/Logotype/Thargelion-White-Icon-150.png");

            //Image from byte[]. Convert to string with Encoding Windows-1252
            var imageAsbyteArrayData = GetImageAsbyteArrayData();
            var dataAsStringToSendToReporter = Encoding.GetEncoding(1252).GetString(imageAsbyteArrayData);
            sampleData.Add("Img2", dataAsStringToSendToReporter);

            var pageSizeInfo = new PageSizeInfo("A4");

            await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, pageSizeInfo, false, true);

            return true;
        }

        private static byte[] GetImageAsbyteArrayData()
        {
            byte[] imageAsbyteArrayData;
            using (var client = new WebClient())
            {
                imageAsbyteArrayData = client.DownloadData("http://www.thargelion.se/Images/Logotype/Thargelion-White-Icon-150.png");
            }
            return imageAsbyteArrayData;
        }
    }
}