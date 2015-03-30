using System.Collections.Generic;
using System.Threading.Tasks;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.SampleConsole.Commands.PdfCommands
{
    public class CreateComplexCommand : ActionCommandBase
    {
        public CreateComplexCommand()
            : base("complex", "create a complex document")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var documentProperties = CreateDocumentProperties(paramList, index);
            var template = CreateTemplate();

            var debug = QueryParam("Debug", GetParam(paramList, index++), () => new List<KeyValuePair<bool, string>> { new KeyValuePair<bool, string>(true, "Yes"), new KeyValuePair<bool, string>(true, "No") });

            PdfCommand.RenderPdf(template, documentProperties, null, debug);

            return true;
        }

        private DocumentProperties CreateDocumentProperties(string paramList, int index)
        {
            var title = QueryParam<string>("Title", GetParam(paramList, index++));
            var author = QueryParam<string>("Author", GetParam(paramList, index++));
            var creator = QueryParam<string>("Creator", GetParam(paramList, index++));
            var subject = QueryParam<string>("Subject", GetParam(paramList, index++));

            var documentProperties = new DocumentProperties
            {
                Title = title,
                Author = author,
                Creator = creator,
                Subject = subject,
            };
            return documentProperties;
        }

        private Template CreateTemplate()
        {
            var firstPageSection = new Section { Name = "First Page" };
            firstPageSection.Pane.ElementList.Add(new TextBox { Top = "50%", Left = "1cm", Value = "Firs page of complex sample document", Font = new Font { Size = 22 } });
            var template = new Template(firstPageSection);

            var indexSection = new Section { Name = "Index", Margin = new UnitRectangle { Left = "2cm", Right = "1cm", Top = "3cm", Bottom = "3cm" } };
            indexSection.Footer.ElementList.Add(new Text { Value = "Page {PageNumber} of {TotalPages}" });
            template.SectionList.Add(indexSection);

            var mainSection = new Section { Name = "Main", Margin = new UnitRectangle { Left = "2cm", Right = "1cm", Top = "3cm", Bottom = "3cm" } };
            mainSection.Header.ElementList.Add(new Text { Value = "{Title}" });
            mainSection.Footer.ElementList.Add(new Text { Value = "Page {PageNumber} of {TotalPages}", TextAlignment = TextBase.Alignment.Left });
            mainSection.Footer.ElementList.Add(new Text { Value = "Page {PageNumber} of {TotalPages}", TextAlignment = TextBase.Alignment.Center });
            mainSection.Footer.ElementList.Add(new Text { Value = "Page {PageNumber} of {TotalPages}", TextAlignment = TextBase.Alignment.Right });
            template.SectionList.Add(mainSection);
            return template;
        }
    }
}