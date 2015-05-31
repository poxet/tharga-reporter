using System.Diagnostics;
using System.Drawing.Printing;
using System.Threading.Tasks;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.PdfCommands
{
    public class PrinterCommand : ContainerCommandBase
    {
        public PrinterCommand() 
            : base("printer")
        {
        }

        public static async Task RenderPrinterAsync(Template template, DocumentProperties documentProperties = null, DocumentData documentData = null, PageSizeInfo pageSizeInfo = null, bool debug = true, bool portrait = true)
        {
            await Task.Factory.StartNew(() => RenderPrinter(template, documentProperties, documentData, pageSizeInfo, debug, portrait));
        }

        private static void RenderPrinter(Template template, DocumentProperties documentProperties = null, DocumentData documentData = null, PageSizeInfo pageSizeInfo = null, bool debug = true, bool portrait = true)
        {
            var renderer = new Renderer(template, documentData, documentProperties, pageSizeInfo, debug);
            var printerSettings = new PrinterSettings { };

            var sw = new Stopwatch();
            sw.Start();

            renderer.Print(printerSettings, true);

            sw.Stop();
            System.Console.WriteLine(sw.Elapsed.TotalMilliseconds.ToString("0.0000"));
        }
    }
}