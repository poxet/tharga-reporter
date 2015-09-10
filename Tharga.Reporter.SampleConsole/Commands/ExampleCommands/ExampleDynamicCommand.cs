using System.Linq;
using System.Threading.Tasks;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class ExampleDynamicCommand : ActionCommandBase
    {
        public ExampleDynamicCommand() 
            : base("dynamic", "Create elements dynamically")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var section = new Section();
            section.Margin = new UnitRectangle { Left = "2cm", Top = "1cm", Bottom = "1cm", Right = "1cm" };
            section.Header.Height = "3cm";
            section.Footer.Height = "3cm";

            var r1 = new ReferencePoint();
            section.Pane.ElementList.Add(r1);
            var cellTitleFont = new Font { Size = 6 };
            var cellDataFont = new Font { Size = 16 };
            r1.ElementList.Add(new Rectangle { Height = "1cm", Width = "60mm" });
            r1.ElementList.Add(new Text { Top = "1mm", Left = "1mm", Value = "Name", Font = cellTitleFont });
            r1.ElementList.Add(new Text { Top = "3mm", Left = "1mm", Value = "{NameData}", Font = cellDataFont });

            r1.ElementList.Add(new Rectangle { Height = "1cm", Width = "60mm", Left = "60mm" });
            r1.ElementList.Add(new Text { Top = "1mm", Left = "61mm", Value = "Address", Font = cellTitleFont });
            r1.ElementList.Add(new Text { Top = "3mm", Left = "61mm", Value = "{AddressData}", Font = cellDataFont });

            r1.ElementList.Add(new Rectangle { Height = "1cm", Width = "20mm", Left = "120mm" });
            r1.ElementList.Add(new Text { Top = "1mm", Left = "121mm", Value = "Country", Font = cellTitleFont });
            r1.ElementList.Add(new Text { Top = "3mm", Left = "121mm", Value = "{CountryData}", Font = cellDataFont });

            //Make a copy and change it...
            var r2 = r1.Clone();
            section.Pane.ElementList.Add(r2);
            r2.Top += "1cm";

            //Create a content box.
            var r3 = new ReferencePoint { Top = "25mm" };
            section.Pane.ElementList.Add(r3);
            r3.ElementList.Add(new Rectangle { Height = "3cm", Width = "10cm" });

            //Create a checkbox with text
            var rc1 = new ReferencePoint();
            r3.ElementList.Add(rc1);
            rc1.ElementList.Add(new Rectangle { Left = "2mm", Width = "5mm", Height = "5mm", Top = "2mm" });
            rc1.ElementList.Add(new Line { Left = "2mm", Width = "5mm", Height = "5mm", Top = "2mm" });
            rc1.ElementList.Add(new Line { Left = "2mm", Width = "5mm", Height = "-5mm", Top = "7mm" });
            rc1.ElementList.Add(new Text { Left = "1cm", Top = "3mm", Value = "This value has been selected.", Font = cellDataFont });

            //Copy the checkbox and change text
            var rc2 = rc1.Clone();
            rc2.Top += "1cm";
            (rc2.ElementList.Last() as Text).Value = "Some other option.";
            r3.ElementList.Add(rc2);

            //Copy the checkbox and change text
            var rc3 = rc2.Clone();
            rc3.Top += "1cm";
            (rc2.ElementList.Last() as Text).Value = "Yet another option.";
            r3.ElementList.Add(rc3);


            var template = new Template(section);
            var documentProperties = new DocumentProperties();
            var sampleData = new DocumentData();
            sampleData.Add("NameData", "Kalle Anka");
            sampleData.Add("AddressData", "Storgatan 1");
            sampleData.Add("CountryData", "SE");

            var pageSizeInfo = new PageSizeInfo("A4");
            await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, pageSizeInfo, false, true);
            return true;
        }
    }
}