using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine.Entity;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.TestCommands
{
    public class TestFileCommand : ActionCommandBase
    {
        private enum output { pdf, printer };

        public TestFileCommand() 
            : base("file", "Test a template and data file")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            var index = 0;
            var templateFile = QueryParam<string>("Template File", GetParam(paramList, index++));
            var dataFile = QueryParam<string>("Data File", GetParam(paramList, index++));
            var debug = QueryParam("Debug", GetParam(paramList, index++), () => new List<KeyValuePair<bool, string>> { new KeyValuePair<bool, string>(true, "Yes"), new KeyValuePair<bool, string>(false, "No") });
            var output = QueryParam("Output", GetParam(paramList, index++), () => new List<KeyValuePair<output, string>> { new KeyValuePair<output, string>(TestFileCommand.output.pdf, TestFileCommand.output.pdf.ToString()), new KeyValuePair<output, string>(TestFileCommand.output.printer, TestFileCommand.output.printer.ToString()) });

            var xmdTemplate = new XmlDocument();
            xmdTemplate.Load(templateFile);
            var template = Template.Load(xmdTemplate);

            var documentProperties = new DocumentProperties();

            var xmdData = new XmlDocument();
            xmdData.Load(dataFile);
            var documentData = DocumentData.Load(xmdData);

            switch (output)
            {
                case output.pdf:
                    await PdfCommand.RenderPdfAsync(template, documentProperties, documentData, null, debug);
                    break;

                case output.printer:
                    await PrinterCommand.RenderPrinterAsync(template, documentProperties, documentData, null, debug);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown output {0}.", output));
            }

            return true;
        }
    }
}