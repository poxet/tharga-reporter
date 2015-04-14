using System.Windows;
using System.Drawing.Printing;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Section = Tharga.Reporter.Engine.Entity.Section;

namespace Tharga.Reporter.WPFSample
{
    using System;
    using System.Printing;

    public partial class MainWindow
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

            DocumentData documentData = null;

            var debug = false;

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

                PageSizeInfo pageSizeInfo = null;
                var pageMediaSize = dialog.PrintTicket.PageMediaSize;
                if (pageMediaSize.PageMediaSizeName != null)
                {
                    try
                    {
                        pageSizeInfo = new PageSizeInfo(pageMediaSize.PageMediaSizeName.Value.ToString());
                    }
                    catch (ArgumentException)
                    {
                        pageSizeInfo = new PageSizeInfo(pageMediaSize.Width / 96 + "inch", pageMediaSize.Height / 96 + "inch");
                    }
                }

                var renderer = new Renderer(template, documentData, documentProperties, pageSizeInfo, debug);

                //Send document to the printer
                renderer.Print(printerSettings);
                
                //Create a pdf for the same document
                var data = renderer.GetPdfBinary();
            }
        }
    }
}