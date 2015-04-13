using System.Threading.Tasks;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class InvoiceArticleNoteCommand : ActionCommandBase
    {
        public InvoiceArticleNoteCommand()
            : base("label", "Create an example artricle label.")
        {
        }

        public async override Task<bool> InvokeAsync(string paramList)
        {
            //var section = new Section { };
            //section.Pane.ElementList.Add(new Text { Value = "My label" });
            //var template = new Template(section);

            //var documentProperties = new DocumentProperties
            //{
            //};

            //var sampleData = new DocumentData();

            ////await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, false);
            //DocumentData documentData = null;

            //var debug = false;

            //var renderer = new Renderer(template, documentData, true, documentProperties, debug);

            ////var dialog = new System.Windows.Controls.PrintDialog
            ////{
            ////    UserPageRangeEnabled = false,
            ////    SelectedPagesEnabled = false,
            ////};

            ////if (dialog.ShowDialog() == true)
            ////{
            ////    var printerSettings = new PrinterSettings
            ////    {
            ////        Copies = (short)(dialog.PrintTicket.CopyCount ?? 1),
            ////        PrinterName = dialog.PrintQueue.FullName,
            ////    };
            ////    renderer.Print(printerSettings);
            ////}

            //var printerSettings = new PrinterSettings
            //{
            //};

            //System.Diagnostics.Debug.WriteLine(printerSettings.PaperSizes.Count);

            //renderer.Print(printerSettings);

            return true;
        }
    }
}