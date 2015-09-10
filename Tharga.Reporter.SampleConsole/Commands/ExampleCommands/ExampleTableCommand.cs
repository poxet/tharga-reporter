using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using Tharga.Reporter.ConsoleSample.Commands.PdfCommands;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Toolkit.Console.Command.Base;

namespace Tharga.Reporter.ConsoleSample.Commands.ExampleCommands
{
    public class ExampleTableCommand : ActionCommandBase
    {
        public ExampleTableCommand()
            : base("table", "Example on how to create a table.")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var section = new Section();
            var table = new Table
            {
                Name = "A", 
                Top = "100", 
                Bottom = "100", 
                Left = "50", 
                Right = "50", 
                
                HeaderBackgroundColor = Color.Pink,
                //HeaderBorderColor = Color.Blue,
                //ColumnBorderColor = Color.Green,
                //ContentBorderColor = Color.DeepPink,
                //GroupBorderColor = Color.Chocolate,

                SkipLine = new SkipLine
                {
                    Height = "0",
                    Interval = 1,
                    BorderColor = Color.Red
                }
            };
            table.AddColumn(new TableColumn { Title = "First", Value = "{A1}" });
            table.AddColumn(new TableColumn { Title = "Second", Value = "{A2}" });
            section.Pane.ElementList.Add(table);
            var template = new Template(section);

            var documentProperties = new DocumentProperties();

            var sampleData = new DocumentData();
            var documentDataTable = new DocumentDataTable("A");
            documentDataTable.AddRow( new Dictionary<string, string> { { "A1", "Some stuff" }, { "A2", "Some stuff" }, });
            documentDataTable.AddRow( new Dictionary<string, string> { { "A1", "Some stuff on the second row" }, { "A2", "Blah" }, });
            documentDataTable.AddRow( new Dictionary<string, string> { { "A1", "And on the third row" }, { "A2", "blah blah blah" }, });
            for(var i = 0; i < 10; i++)
                documentDataTable.AddRow(new Dictionary<string, string> { { "A1", i.ToString() }, { "A2", "hästmos" }, });

            sampleData.Add(documentDataTable);

            var pageSizeInfo = new PageSizeInfo("A4");

            await PdfCommand.RenderPdfAsync(template, documentProperties, sampleData, pageSizeInfo, false, true);

            return true;
        }
    }
}