using System.Threading.Tasks;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.PdfCommands
{
    public class CreateMutiSection : ActionCommandBase
    {
        public CreateMutiSection() 
            : base("section", "Creates a document with multiple sections")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            var sectionA = new Section { Name = "Section A" };
            sectionA.Pane.ElementList.Add(new Text { Font = new Font { Size = 22 }, Top = "50%", TextAlignment = TextBase.Alignment.Center, Value = "Multi section demo" });
            var template = new Template(sectionA);

            var sectionB = new Section { Name = "Section B", Margin = new UnitRectangle { Left = "2cm", Right = "1cm", Top = "1cm", Bottom = "1cm" }, Footer = { Height = "3cm" } };
            sectionB.Footer.ElementList.Add(new Text { Value = "Some text on page {PageNumber}." });
            sectionB.Footer.ElementList.Add(new Text { Left = "5cm", Value = "Some text on the last page.", Visibility = PageVisibility.LastPage });
            template.SectionList.Add(sectionB);

            var sectionC = new Section { Name = "Section C", Margin = new UnitRectangle { Left = "2cm", Right = "1cm", Top = "1cm", Bottom = "1cm" }, Footer = sectionB.Footer };
            sectionC.Pane.ElementList.Add(new TextBox { Width = "3cm", Height = "2cm", Value = "This is a text that spans over several pages. The first part should appear on one page and the last part on another." });
            sectionC.Pane.ElementList.Add(new TextBox { Left = "5cm", Width = "3cm", Height = "2cm", Value = "This is a short text." });
            sectionC.Pane.ElementList.Add(new TextBox { Left = "10cm", Width = "3cm", Height = "2cm", Value = "This is a short text for the last page only.", Visibility = PageVisibility.LastPage });
            sectionC.Pane.ElementList.Add(new TextBox { Left = "15cm", Width = "3cm", Height = "2cm", Value = "This is a text for all pages but for the first one.", Visibility = PageVisibility.AllButFirst });
            template.SectionList.Add(sectionC);

            await PdfCommand.RenderPdfAsync(template);

            return true;
        }
    }
}