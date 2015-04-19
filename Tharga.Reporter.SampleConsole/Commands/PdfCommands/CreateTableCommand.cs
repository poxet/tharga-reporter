using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Toolkit.Console.Command.Base;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.ConsoleSample.Commands.PdfCommands
{
    public class CreateTableCommand : ActionCommandBase
    {
        public CreateTableCommand()
            : base("table", "Crate a simple table")
        {
        }

        public override async Task<bool> InvokeAsync(string paramList)
        {
            var section = CreateSection();
            var template = new Template(section);

            var table = CreateTableDefinition();
            section.Pane.ElementList.Add(table);

            //Add some column lines for the table
            section.Pane.ElementList.Add(new Line { Left = "4cm", Width = "0", Color = Color.Gray, Top = "10%", Height = "80%" });
            section.Pane.ElementList.Add(new Line { Left = "8cm", Width = "0", Color = Color.Gray, Top = "10%", Height = "80%" });

            var data = GetData();

            var documentProperties = CreateDocumentProperties();

            await PdfCommand.RenderPdfAsync(template, documentProperties, data, null, false);

            return true;
        }

        private static DocumentProperties CreateDocumentProperties()
        {
            var documentProperties = new DocumentProperties { Title = "Table example" };
            return documentProperties;
        }

        private static Section CreateSection()
        {
            var section = new Section
            {
                Name = "Main section",
                Margin = new UnitRectangle { Left = "3cm", Top = "1cm", Right = "1cm", Bottom = "1cm" },
                Header = { Height = "3cm" },
            };

            //Header
            section.Header.ElementList.Add(new Text { Value = "{Title}" });
            section.Header.ElementList.Add(new Text
            {
                Value = "{SomeOtherData}",
                TextAlignment = TextBase.Alignment.Right
            });

            //Footer
            section.Footer.Height = "3cm";
            section.Footer.ElementList.Add(new Text
            {
                Value = "Page {PageNumber} of {TotalPages}",
                TextAlignment = TextBase.Alignment.Center
            });

            //Total sum text on the last page
            section.Pane.ElementList.Add(new Text
            {
                Value = "Total ${TotalSum}",
                Top = "92%",
                Left = "4cm",
                Width = "4cm",
                TextAlignment = TextBase.Alignment.Right,
                Visibility = PageVisibility.LastPage
            });

            return section;
        }

        private static Table CreateTableDefinition()
        {
            var table = new Table
            {
                Name = "MyTable",
                Left = "1cm",
                Top = "10%",
                Right = "1cm",
                Height = "80%",
                HeaderBorderColor = Color.Black,
                HeaderFont = new Font { Bold = true },
                SkipLine = new SkipLine(),
                ContentBorderColor = Color.Gray,
            };
            table.AddColumn(new TableColumn { Value = "{FirstCol}", Title = "First column", Width = "3cm", WidthMode = Table.WidthMode.Specific });
            table.AddColumn(new TableColumn { Value = "${SecondCol}", Title = "Second column", Width = "4cm", WidthMode = Table.WidthMode.Specific, Align = Table.Alignment.Right });
            table.AddColumn(new TableColumn { Value = "{ThirdCol}", Title = "Third column", WidthMode = Table.WidthMode.Spring });
            return table;
        }

        private static DocumentData GetData()
        {
            var data = new DocumentData();

            //Prepare the data for the table
            var documentDataTable = new DocumentDataTable("MyTable");
            var totalSum = 0;
            for (var i = 0; i < 100; i++)
            {
                var val = i * 70;
                totalSum += val;

                documentDataTable.AddRow(new Dictionary<string, string>
                {
                    { "FirstCol", "Col 1 Row " + i },
                    { "SecondCol", val.ToString("#,##0.00") },
                    { "ThirdCol", new string((char)(65 + (i / 4)), 1 + (i % 20)) }
                });
            }

            data.Add(documentDataTable);

            //Prepare some other data
            data.Add("SomeOtherData", "This is some custom data");
            data.Add("TotalSum", totalSum.ToString("#,##0.00"));

            return data;
        }
    }
}