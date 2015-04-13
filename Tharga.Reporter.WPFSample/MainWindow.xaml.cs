using System.Windows;
using System.Drawing.Printing;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Section = Tharga.Reporter.Engine.Entity.Section;

namespace Tharga.Reporter.WPFSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var section = new Section { };
            section.Pane.ElementList.Add(new Text { Value = "My label" });
            var template = new Template(section);

            var documentProperties = new DocumentProperties
            {
            };

            var sampleData = new DocumentData();

            //await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, false);
            DocumentData documentData = null;

            var debug = false;

            var renderer = new Renderer(template, documentData, true, documentProperties, debug);

            var dialog = new System.Windows.Controls.PrintDialog
            {
                UserPageRangeEnabled = false,
                SelectedPagesEnabled = false,
            };

            if (dialog.ShowDialog() == true)
            {
                var printerSettings = new PrinterSettings
                {
                    Copies = (short)(dialog.PrintTicket.CopyCount ?? 1),
                    PrinterName = dialog.PrintQueue.FullName,
                };
                renderer.Print(printerSettings);
            }
        }
    }
}
