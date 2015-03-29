using System.Diagnostics;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.SampleConsole.Commands.PdfCommands
{
    public class PdfCommand : ContainerCommandBase
    {
        public PdfCommand() 
            : base("pdf")
        {
            RegisterCommand(new CreateBlankCommand());
            RegisterCommand(new CreateComplexCommand());
        }

        public static void RenderPdf(Template template, DocumentProperties documentProperties = null, DocumentData documentData = null, bool debug = true)
        {
            var renderer = new Renderer(template, documentData, true, documentProperties, debug);
            var bytes = renderer.GetPdfBinary();
            ExecuteFile(bytes);
        }

        public static void ExecuteFile(byte[] byteArray)
        {
            var fileName = string.Format("{0}.pdf", System.IO.Path.GetTempFileName());
            System.IO.File.WriteAllBytes(fileName, byteArray);
            Process.Start(fileName);

            System.Threading.Thread.Sleep(5000);

            while (System.IO.File.Exists(fileName))
            {
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch (System.IO.IOException)
                {
                    System.Console.WriteLine("Waiting for the document to close before it can be deleted...");
                    System.Threading.Thread.Sleep(5000);
                }
            }
        }
    }
}
