using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MigraDoc.Rendering;
using Moq;
using NUnit.Framework;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;
using Font = Tharga.Reporter.Engine.Entity.Font;

namespace Tharga.Reporter.Tests.Serializing
{
    [TestFixture]
    public class Table_Tests
    {
        [Test]
        public void Default_Table()
        {
            //Arrange
            var table = new Table();
            var xme = table.ToXme();

            //Act
            var otherLine = Table.Load(xme);

            //Assert
            Assert.AreEqual(table.Left, otherLine.Left);
            Assert.AreEqual(table.Right, otherLine.Right);
            Assert.AreEqual(table.Width, otherLine.Width);
            Assert.AreEqual(table.Top, otherLine.Top);
            Assert.AreEqual(table.Bottom, otherLine.Bottom);
            Assert.AreEqual(table.Height, otherLine.Height);
            Assert.AreEqual(table.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(table.Name, otherLine.Name);
            Assert.AreEqual(table.ContentBackgroundColor, otherLine.ContentBackgroundColor);
            Assert.AreEqual(table.ContentBorderColor, otherLine.ContentBorderColor);
            Assert.AreEqual(table.HeaderBackgroundColor, otherLine.HeaderBackgroundColor);
            Assert.AreEqual(table.HeaderBorderColor, otherLine.HeaderBorderColor);
            Assert.AreEqual(table.HeaderFontClass, otherLine.HeaderFontClass);
            Assert.AreEqual(table.HeaderFont.Size, otherLine.HeaderFont.Size);
            Assert.AreEqual(table.ContentFontClass, otherLine.ContentFontClass);
            Assert.AreEqual(table.ContentFont.FontName, otherLine.ContentFont.FontName);
            Assert.AreEqual(table.SkipLine, otherLine.SkipLine);
            //This is throwing an exception due to the skipline being null
            //Assert.AreEqual(table.SkipLine.BorderColor, otherLine.SkipLine.BorderColor);
            Assert.AreEqual(table.ColumnPadding, otherLine.ColumnPadding);
            Assert.AreEqual(table.RowPadding, otherLine.RowPadding);
            Assert.AreEqual(table.ToString(), otherLine.ToString());
            Assert.AreEqual(table.Columns.Count, otherLine.Columns.Count);
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        public void Table_with_all_properties_set()
        {
            //Arrange
            var table = new Table
            {
                Bottom = UnitValue.Parse("10px"),
                Height = UnitValue.Parse("20px"),
                IsBackground = true,
                Left = UnitValue.Parse("10cm"),
                Right = UnitValue.Parse("20cm"),
                Name = "Bob",

                ContentBackgroundColor = Color.Navy,
                ContentBorderColor = Color.Olive,
                ContentFont = new Font { FontName = "Times", Size = 7 },

                HeaderBackgroundColor = Color.MediumTurquoise,
                HeaderBorderColor = Color.MediumVioletRed,
                HeaderFontClass = "A",

                GroupBackgroundColor = Color.Plum,
                GroupBorderColor = Color.Red,
                GroupSpacing = "5mm",
                GroupFont = new Font { Bold = true, Color = Color.Red, FontName = "Times", Size = 12, },

                SkipLine = new SkipLine { Interval = 5, Height = "8mm", BorderColor = Color.Red},
                ColumnPadding = UnitValue.Parse("7mm"),
                RowPadding = UnitValue.Parse("6mm"),
            };
            //table.AddColumn(new TableColumn { Value = "A0", Title = "B", Width = "1cm", Align = Table.Alignment.Right, HideValue = "123", WidthMode = Table.WidthMode.Spring });
            table.AddColumn("A0", "B", "1cm", alignment: Table.Alignment.Right, hideValue: "123", widthMode: Table.WidthMode.Spring);
            table.AddColumn(new TableColumn { Value = "A2", Title = "B", Width = "1cm", Align = Table.Alignment.Right, HideValue = "123", WidthMode = Table.WidthMode.Spring });
            table.AddColumn(new TableColumn { Value = "A1", Title = "B", Width = "1cm", Align = Table.Alignment.Right, HideValue = "123", WidthMode = Table.WidthMode.Spring });
            var xme = table.ToXme();

            //Act
            var otherLine = Table.Load(xme);

            //Assert
            Assert.AreEqual(table.Left, otherLine.Left);
            Assert.AreEqual(table.Right, otherLine.Right);
            Assert.AreEqual(table.Width, otherLine.Width);
            Assert.AreEqual(table.Top, otherLine.Top);
            Assert.AreEqual(table.Bottom, otherLine.Bottom);
            Assert.AreEqual(table.Height, otherLine.Height);
            Assert.AreEqual(table.IsBackground, otherLine.IsBackground);
            Assert.AreEqual(table.Name, otherLine.Name);

            Assert.AreEqual(table.ContentBackgroundColor.Value.ToArgb(), otherLine.ContentBackgroundColor.Value.ToArgb());
            Assert.AreEqual(table.ContentBorderColor.Value.ToArgb(), otherLine.ContentBorderColor.Value.ToArgb());
            Assert.AreEqual(table.ContentFontClass, otherLine.ContentFontClass);
            Assert.AreEqual(table.ContentFont.FontName, otherLine.ContentFont.FontName);

            Assert.AreEqual(table.HeaderBackgroundColor.Value.ToArgb(), otherLine.HeaderBackgroundColor.Value.ToArgb());
            Assert.AreEqual(table.HeaderBorderColor.Value.ToArgb(), otherLine.HeaderBorderColor.Value.ToArgb());
            Assert.AreEqual(table.HeaderFontClass, otherLine.HeaderFontClass);
            Assert.AreEqual(table.HeaderFont.Size, otherLine.HeaderFont.Size);

            Assert.AreEqual(table.GroupBackgroundColor.Value.ToArgb(), otherLine.GroupBackgroundColor.Value.ToArgb());
            Assert.AreEqual(table.GroupBorderColor.Value.ToArgb(), otherLine.GroupBorderColor.Value.ToArgb());
            Assert.AreEqual(table.GroupSpacing.Value, otherLine.GroupSpacing.Value);
            Assert.AreEqual(table.GroupFont.FontName, otherLine.GroupFont.FontName);
            Assert.AreEqual(table.GroupFont.Size, otherLine.GroupFont.Size);

            Assert.AreEqual(table.SkipLine.Interval, otherLine.SkipLine.Interval);
            Assert.AreEqual(table.SkipLine.Height, otherLine.SkipLine.Height);
            Assert.AreEqual(table.SkipLine.BorderColor.Value.ToArgb(), otherLine.SkipLine.BorderColor.Value.ToArgb());
            Assert.AreEqual(table.ColumnPadding, otherLine.ColumnPadding);
            Assert.AreEqual(table.RowPadding, otherLine.RowPadding);
            Assert.AreEqual(table.ToString(), otherLine.ToString());
            Assert.AreEqual(table.Columns.Count, otherLine.Columns.Count);
            Assert.AreEqual(xme.OuterXml, otherLine.ToXme().OuterXml);
        }

        [Test]
        [Ignore("Can't gain access to internal stuff.")]
        public void When_rendering_an_empty_document_with_debugging()
        {
            //Arrange
            var sectionName = "ABC123";
            var section = new Section { Name = sectionName };
            var template = new Template(section);
            var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize());
            graphicsMock.Setup(x => x.DrawString(sectionName, It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>()));
            graphicsMock.Setup(x => x.DrawLine(It.IsAny<XPen>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>(), It.IsAny<double>()));
            var graphicsFactoryMock = new Mock<IGraphicsFactory>(MockBehavior.Strict);
            graphicsFactoryMock.Setup(x => x.PrepareGraphics(It.IsAny<PdfPage>(), It.IsAny<DocumentRenderer>(), It.IsAny<int>())).Returns(graphicsMock.Object);
            var renderer = new Renderer(graphicsFactoryMock.Object, template, null, null, null, true);

            //Act
            var data = renderer.GetPdfBinary();

            //Assert
            Assert.Greater(data.Length, 0);
        }

        [Test]
        public void When_rendering_a_document_with_an_empty_table()
        {
            //Arrange
            var table = new Table();
            var section = new Section();
            section.Pane.ElementList.Add(table);
            var template = new Template(section);
            var documentData = new DocumentData();
            var renderer = new Renderer(template, documentData);

            //Act
            var data = renderer.GetPdfBinary();

            //Assert
        }

        [Test]
        [Ignore("Can't gain access to internal stuff.")]
        public void When_rendering_several_times_with_the_same_template()
        {
            //Arrange
            var table1 = new Table { Name = "TableA", Top = "2cm", Height = "5cm" };
            table1.AddColumn(new TableColumn { Value = "ColumnA1", Title = "Column A", Width = "2cm", WidthMode = Table.WidthMode.Auto, Align = Table.Alignment.Left, HideValue = string.Empty });
            var section = new Section();
            section.Pane.ElementList.Add(table1);
            var template = new Template(section);
            var documentData1 = new DocumentData();
            var tbl1 = new DocumentDataTable("TableA");
            documentData1.Add(tbl1);
            for (var i = 0; i < 30; i++)
            {
                var row1 = new Dictionary<string, string>();
                row1.Add("ColumnA1", "DataA" + i);
                tbl1.AddRow(row1);
            }

            var documentData2 = new DocumentData();
            var tbl2 = new DocumentDataTable("TableA");
            documentData2.Add(tbl2);
            for (var i = 0; i < 10; i++)
            {
                var row2 = new Dictionary<string, string>();
                row2.Add("ColumnA1", "DataA" + i);
                tbl2.AddRow(row2);
            }

            var documentData3 = new DocumentData();
            var tbl3 = new DocumentDataTable("TableA");
            documentData3.Add(tbl3);
            for (var i = 0; i < 20; i++)
            {
                var row3 = new Dictionary<string, string>();
                row3.Add("ColumnA1", "DataA" + i);
                tbl3.AddRow(row3);
            }

            var graphicsMock = new Mock<IGraphics>(MockBehavior.Strict);
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>())).Returns(new XSize());
            graphicsMock.Setup(x => x.MeasureString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XStringFormat>())).Returns(new XSize());
            graphicsMock.Setup(x => x.DrawString(It.IsAny<string>(), It.IsAny<XFont>(), It.IsAny<XBrush>(), It.IsAny<XPoint>(), It.IsAny<XStringFormat>()));

            var graphicsFactoryMock = new Mock<IGraphicsFactory>(MockBehavior.Strict);
            graphicsFactoryMock.Setup(x => x.PrepareGraphics(It.IsAny<PdfPage>(), It.IsAny<DocumentRenderer>(), It.IsAny<int>())).Returns(graphicsMock.Object);

            var renderer1 = new Renderer(graphicsFactoryMock.Object, template, documentData1);
            var renderer2 = new Renderer(graphicsFactoryMock.Object, template, documentData2);
            var renderer3 = new Renderer(graphicsFactoryMock.Object, template, documentData3);

            //Act
            var data1 = renderer1.GetPdfBinary();
            var data2 = renderer2.GetPdfBinary();
            var data3 = renderer3.GetPdfBinary();

            //Assert
            //Assert.AreEqual(data1, data2);
        }

        [Test]
        public void When_serializing_and_deserializing_data_with_group_lines()
        {
            //Arrange
            var originalDocumentData = new DocumentData();
            var tableData = new DocumentDataTable("A");
            tableData.AddGroup("Group1");
            tableData.AddRow(new Dictionary<string, string> { { "Col1", "Data1A" }, { "Col2", "Data2A" } });
            tableData.AddRow(new Dictionary<string, string> { { "Col1", "Data1B" }, { "Col2", "Data2B" } });
            tableData.AddGroup("Group2");
            tableData.AddRow(new Dictionary<string, string> { { "Col1", "Data1C" }, { "Col2", "Data2C" } });
            originalDocumentData.Add(tableData);
            var xmlDocumentData = originalDocumentData.ToXml();

            //Act
            var loadedDocumentData = DocumentData.Load(xmlDocumentData);

            //Assert
            Assert.AreEqual(originalDocumentData.GetDataTable("A").Rows.Count, loadedDocumentData.GetDataTable("A").Rows.Count);
            Assert.AreEqual(originalDocumentData.GetDataTable("A").Rows.Count(x => x is DocumentDataTableData), loadedDocumentData.GetDataTable("A").Rows.Count(x => x is DocumentDataTableData));
            Assert.AreEqual(originalDocumentData.GetDataTable("A").Rows.Count(x => x is DocumentDataTableGroup), loadedDocumentData.GetDataTable("A").Rows.Count(x => x is DocumentDataTableGroup));
            Assert.AreEqual((originalDocumentData.GetDataTable("A").Rows.First(x => x is DocumentDataTableGroup) as DocumentDataTableGroup).Content, (loadedDocumentData.GetDataTable("A").Rows.First(x => x is DocumentDataTableGroup) as DocumentDataTableGroup).Content);
            Assert.AreEqual((originalDocumentData.GetDataTable("A").Rows.Last(x => x is DocumentDataTableGroup) as DocumentDataTableGroup).Content, (loadedDocumentData.GetDataTable("A").Rows.Last(x => x is DocumentDataTableGroup) as DocumentDataTableGroup).Content);
            Assert.AreEqual((originalDocumentData.GetDataTable("A").Rows.First(x => x is DocumentDataTableData) as DocumentDataTableData).Columns.First().Key, (loadedDocumentData.GetDataTable("A").Rows.First(x => x is DocumentDataTableData) as DocumentDataTableData).Columns.First().Key);
            Assert.AreEqual((originalDocumentData.GetDataTable("A").Rows.First(x => x is DocumentDataTableData) as DocumentDataTableData).Columns.First().Value, (loadedDocumentData.GetDataTable("A").Rows.First(x => x is DocumentDataTableData) as DocumentDataTableData).Columns.First().Value);
        }
    }
}