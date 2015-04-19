using System.Threading.Tasks;
using System.Xml;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine.Entity;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class TestFileCommand : ActionCommandBase
    {
        public TestFileCommand() 
            : base("file", "Test a template and data file")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var templateFile = QueryParam<string>("Template File", GetParam(paramList, index++));
            var dataFile = QueryParam<string>("Data File", GetParam(paramList, index++));

            var xmdTemplate = new XmlDocument();
            xmdTemplate.Load(templateFile);
            var template = Template.Load(xmdTemplate);

            var documentProperties = new DocumentProperties();

            var xmdData = new XmlDocument();
            xmdData.Load(dataFile);
            var documentData = DocumentData.Load(xmdData);

            await PdfCommand.RenderPdfAsync(template, documentProperties, documentData, null, true);

            return true;
        }
    }

    public class TestCommands : ContainerCommandBase
    {
        public TestCommands()
            : base("test")
        {
            RegisterCommand(new TestFileCommand());
        }
    }

    public class ExampleCommands : ContainerCommandBase
    {
        public ExampleCommands() 
            : base("example")
        {
            RegisterCommand(new ExampleInvoiceCommand());
            RegisterCommand(new ExampleArticleNoteCommand());
        }
    }
}