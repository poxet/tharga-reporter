using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Security;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;
using Section = Tharga.Reporter.Engine.Entity.Section;

namespace Tharga.Reporter.Engine
{
    public enum PrinterInteractionMode { None, Active }

    public class Renderer
    {
        private readonly Template _template;
        private readonly DocumentData _documentData;
        private readonly DocumentProperties _documentProperties;
        private readonly PrinterInteractionMode _printerInteractionMode;
        private readonly IDebugData _debugData;
        private readonly IGraphicsFactory _graphicsFactory;
        private readonly PageSizeInfo _pageSizeInfo;
        private int _printPageCount;
        private bool _preRendered;
        private bool _includeBackgroundObjects = true;

        internal Renderer(IGraphicsFactory graphicsFactory, Template template, DocumentData documentData = null, DocumentProperties documentProperties = null, PageSizeInfo pageSizeInfo = null, bool debug = false, PrinterInteractionMode printerInteractionMode = PrinterInteractionMode.None)
        {
            _template = template;
            _documentData = documentData;
            _documentProperties = documentProperties;
            _printerInteractionMode = printerInteractionMode;
            _pageSizeInfo = pageSizeInfo ?? new PageSizeInfo(PageSize.A4);
            if (debug)
                _debugData = new DebugData();
            _graphicsFactory = graphicsFactory;
        }

        public Renderer(Template template, DocumentData documentData = null, DocumentProperties documentProperties = null, PageSizeInfo pageSizeInfo = null, bool debug = false, PrinterInteractionMode printerInteractionMode = PrinterInteractionMode.None)
            : this(new MyGraphicsFactory(), template, documentData, documentProperties, pageSizeInfo, debug, printerInteractionMode)
        {
        }

        public void CreatePdfFile(string fileName, bool includeBackgroundObjects = true)
        {
            if (File.Exists(fileName))
                throw new InvalidOperationException("The file already exists.").AddData("fileName", fileName);

            File.WriteAllBytes(fileName, GetPdfBinary(includeBackgroundObjects));
        }

        public byte[] GetPdfBinary(bool includeBackgroundObjects = true, bool portrait = true)
        {
            PreRender(_pageSizeInfo, includeBackgroundObjects, portrait);

            var pdfDocument = CreatePdfDocument();
            RenderPdfDocument(pdfDocument, false, _pageSizeInfo, includeBackgroundObjects, portrait);

            var memStream = new MemoryStream();
            pdfDocument.Save(memStream);
            return memStream.ToArray();
        }

        public void Print(PrinterSettings printerSettings, bool includeBackgroundObjects = true)
        {
            _printPageCount = 0;
            _includeBackgroundObjects = includeBackgroundObjects;

            var pageSizeInfo = GetPageSizeInfo(printerSettings);

            var portrait = false;
            if (_printerInteractionMode == PrinterInteractionMode.Active)
            {
                portrait = !printerSettings.DefaultPageSettings.Landscape;
            }

            PreRender(pageSizeInfo, includeBackgroundObjects, portrait);

            var doc = GetDocument(false, portrait);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            var printDocument = new MigraDocPrintDocument();
            printDocument.PrintController = new StandardPrintController();

            printDocument.PrintPage += PrintDocument_PrintPage;
            printDocument.Renderer = docRenderer;
            printDocument.PrinterSettings = printerSettings;
            printDocument.Print();
        }

        private PageSizeInfo GetPageSizeInfo(PrinterSettings printerSettings)
        {
            PageSizeInfo pageSizeInfo;

            switch (_printerInteractionMode)
            {
                case PrinterInteractionMode.None:
                    pageSizeInfo = new PageSizeInfo(_pageSizeInfo.PageSize);
                    break;

                case PrinterInteractionMode.Active:
                    try
                    {
                        var paperKind = printerSettings.DefaultPageSettings.PaperSize.Kind;
                        if (paperKind.ToString() == "Custom")
                        {
                            pageSizeInfo = GetDefaultPageSizeInfo(printerSettings);
                        }
                        else
                        {
                            pageSizeInfo = new PageSizeInfo(paperKind.ToString());
                        }
                    }
                    catch (ArgumentException)
                    {
                        pageSizeInfo = GetDefaultPageSizeInfo(printerSettings);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unknown printer interaction mode {0}.", _printerInteractionMode));
            }


            return pageSizeInfo;
        }

        private static PageSizeInfo GetDefaultPageSizeInfo(PrinterSettings printerSettings)
        {
            return new PageSizeInfo(new UnitValue((printerSettings.DefaultPageSettings.PaperSize.Width / 96), UnitValue.EUnit.Inch), new UnitValue((printerSettings.DefaultPageSettings.PaperSize.Height / 96), UnitValue.EUnit.Inch));
        }

        private void RenderPdfDocument(PdfDocument pdfDocument, bool preRender, PageSizeInfo pageSizeInfo, bool includeBackgroundObjects, bool portrait)
        {
            if (_preRendered && preRender)
                throw new InvalidOperationException("Prerender has already been performed.");

            var doc = GetDocument(preRender, portrait);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            for (var ii = 0; ii < doc.Sections.Count; ii++)
            {
                var page = AddPage(pdfDocument, pageSizeInfo, portrait);

                var gfx = _graphicsFactory.PrepareGraphics(page, docRenderer, ii);

                DoRenderStuff(gfx, new XRect(0, 0, page.Width, page.Height), preRender, ii, _template.SectionList.Sum(x => x.GetRenderPageCount()), includeBackgroundObjects, new XRect(0, 0, 0, 0));
            }

            if (preRender)
            {
                _preRendered = true;
            }
        }

        private static PdfPage AddPage(PdfDocument pdfDocument, PageSizeInfo pageSizeInfo, bool portrait)
        {
            var page = pdfDocument.AddPage();

            if (pageSizeInfo.IsCustomSize)
            {
                page.Width = pageSizeInfo.Width;
                page.Height = pageSizeInfo.Height;
            }
            else
            {
                page.Size = pageSizeInfo.PageSize;
            }

            if (!portrait)
                page.Rotate = 270;

            return page;
        }

        private PdfDocument CreatePdfDocument()
        {
            var pdfDocument = new PdfDocument();
            if (_documentProperties != null)
            {
                pdfDocument.Info.Author = _documentProperties.Author;
                pdfDocument.Info.Subject = _documentProperties.Subject;
                pdfDocument.Info.Title = _documentProperties.Title;
                pdfDocument.Info.Creator = _documentProperties.Creator;
            }

            //TODO: Add other properties as well
            //pdfDocument.Info.Keywords 
            //pdfDocument.Info.Owner 
            //pdfDocument.Info.Producer 
            //pdfDocument.PageLayout = PdfSharp.Pdf.PdfPageLayout.OneColumn 
            //pdfDocument.PageMode = PdfSharp.Pdf.PdfPageMode.FullScreen 
            //pdfDocument.Language 
            //pdfDocument.Outlines
            //pdfDocument.SecurityHandler 
            //pdfDocument.SecuritySettings 
            //pdfDocument.Settings.PrivateFontCollection 
            //pdfDocument.Version 
            //pdfDocument.ViewerPreferences.

            //TODO: Provide security settings
            pdfDocument.SecuritySettings.DocumentSecurityLevel = PdfDocumentSecurityLevel.Encrypted128Bit;
            pdfDocument.SecuritySettings.OwnerPassword = "qwerty12";
            pdfDocument.SecuritySettings.PermitAccessibilityExtractContent = false;
            pdfDocument.SecuritySettings.PermitAnnotations = false;
            pdfDocument.SecuritySettings.PermitAssembleDocument = false;
            pdfDocument.SecuritySettings.PermitExtractContent = true; //Is this copy-paste block?
            pdfDocument.SecuritySettings.PermitFormsFill = false;
            pdfDocument.SecuritySettings.PermitFullQualityPrint = true;
            pdfDocument.SecuritySettings.PermitModifyDocument = false;
            pdfDocument.SecuritySettings.PermitPrint = true;
            ////pdfDocument.SecuritySettings.UserPassword = "qwerty";
            return pdfDocument;
        }

        private void PreRender(PageSizeInfo pageSizeInfo, bool includeBackgroundObjects, bool portrait)
        {
            //TODO: If prerender with one format (pageSize) and printing with another.
            //or, if template or document data changed between render and pre-render then things will be messed up.
            if (!_preRendered)
            {
                var hasMultiPageElements = _template.SectionList.Any(x => x.Pane.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement) || x.Header.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement) || x.Footer.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement));
                if (hasMultiPageElements)
                {
                    var pdfDocument = CreatePdfDocument();
                    RenderPdfDocument(pdfDocument, true, pageSizeInfo, includeBackgroundObjects, portrait);
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var page = _printPageCount++;

            var rawSize = GetSize(e);
            var unitSize = GetSize(rawSize);
            var scale = GetScale(unitSize);
            var actualPaperMargin = GetActual(e);

            var gfx = XGraphics.FromGraphics(e.Graphics, rawSize, XGraphicsUnit.Point);
            gfx.ScaleTransform(scale);

            DoRenderStuff(new MyGraphics(gfx), new XRect(unitSize), false, page, _template.SectionList.Sum(x => x.GetRenderPageCount()), _includeBackgroundObjects, actualPaperMargin);
        }

        private XRect GetActual(PrintPageEventArgs e)
        {
            if (_printerInteractionMode == PrinterInteractionMode.None)
            {
                return new XRect();
            }

            var printableArea = e.PageSettings.PrintableArea;

            var l = printableArea.Left;
            var t = printableArea.Top;
            var r = e.PageBounds.Width - printableArea.Width - l;
            var b = e.PageBounds.Height - printableArea.Height - t;

            if (printableArea.Width < printableArea.Height)
            {
                l = printableArea.Top;
                t = printableArea.Left;
                r = e.PageBounds.Width - printableArea.Height - l;
                b = e.PageBounds.Height - printableArea.Width - t;
            }

            if (b < 0 || r < 0)
            {
                return new XRect();
            }

            var sz2 = new UnitRectangle((l / 96) + "inch", (t / 96) + "inch", (r / 96) + "inch", (b / 96) + "inch");
            var actualPaperMargin = sz2.ToXRect();
            return actualPaperMargin;
        }

        private static XSize GetSize(XSize rawSize)
        {
            var wm = PrinterUnitConvert.Convert(rawSize.Width, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter) / 100;
            var wx = new XUnit(wm, XGraphicsUnit.Millimeter);
            var hm = PrinterUnitConvert.Convert(rawSize.Height, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter) / 100;
            var hx = new XUnit(hm, XGraphicsUnit.Millimeter);
            var unitSize = new XSize(wx, hx);
            return unitSize;
        }

        private static double GetScale(XSize size)
        {
            var wm = PrinterUnitConvert.Convert(size.Width, PrinterUnit.Display, PrinterUnit.HundredthsOfAMillimeter) / 100;
            var wx = new XUnit(wm, XGraphicsUnit.Millimeter);
            var scale = size.Width / wx.Point;
            return scale;
        }

        private static XSize GetSize(PrintPageEventArgs e)
        {
            //margin.Left.Value.GetXUnitValue(XGraphicsUnit.Millimeter)
            //If SectionMargin < ActualMargin (on any side), use actual margin only.
            //If SectionMargin > ActualMargin 

            //No margins, use the maximum printable area of the paper
            //var w = e.PageSettings.PrintableArea.Width;
            //var h = e.PageSettings.PrintableArea.Height;
            //var size = e.PageSettings.Landscape ? new XSize(h, w) : new XSize(w, h);

            //Margins has been defined, use the entire paper
            var size = new XSize(e.PageBounds.Width, e.PageBounds.Height);

            return size;
        }

        private void DoRenderStuff(IGraphics gfx, XRect size, bool preRender, int page, int? totalPages, bool includeBackgroundObjects, XRect actualPaperMargin)
        {
            var postRendering = new List<Action>();

            var pageNumberInfo = new PageNumberInfo(page + 1, totalPages);

            var section = GetSection(preRender, page);

            var sectionBounds = new XRect(section.Margin.GetLeft(size.Width) - actualPaperMargin.Left, section.Margin.GetTop(size.Height) - actualPaperMargin.Top, section.Margin.GetWidht(size.Width), section.Margin.GetHeight(size.Height));

            if (_debugData != null)
            {
                var sectionName = string.IsNullOrEmpty(section.Name) ? "Unnamed section" : section.Name;
                var textSize = gfx.MeasureString(sectionName, _debugData.Font);
                gfx.DrawString(sectionName, _debugData.Font, _debugData.Brush, new XPoint(0, textSize.Height));

                //Left margin
                gfx.DrawLine(_debugData.Pen, sectionBounds.Left, 0, sectionBounds.Left, size.Height);

                //Right margin
                gfx.DrawLine(_debugData.Pen, sectionBounds.Right, 0, sectionBounds.Right, size.Height);

                //Top margin
                gfx.DrawLine(_debugData.Pen, 0, sectionBounds.Top, size.Width, sectionBounds.Top);

                //Bottom margin
                gfx.DrawLine(_debugData.Pen, 0, sectionBounds.Bottom, size.Width, sectionBounds.Bottom);
            }

            var headerHeight = section.Header.Height.ToXUnit(sectionBounds.Height);
            var footerHeight = section.Footer.Height.ToXUnit(sectionBounds.Height);
            var paneBounds = new XRect(sectionBounds.Left, sectionBounds.Top + headerHeight, sectionBounds.Width, sectionBounds.Height - headerHeight - footerHeight);

            var renderData = new RenderData(gfx, paneBounds, section, _documentData, pageNumberInfo, _debugData, includeBackgroundObjects, _documentProperties);

            if (preRender)
            {
                var pageCountPane = section.Pane.PreRender(renderData);
                var pageCountFooter = section.Footer.PreRender(renderData);
                var pageCountHeader = section.Header.PreRender(renderData);
                var pageCount = Max(pageCountPane, pageCountFooter, pageCountHeader);
                section.SetRenderPageCount(pageCount);
            }
            else
            {
                section.Pane.Render(renderData, page);

                //Header
                if (section.Header != null)
                {
                    if (Math.Abs(headerHeight) < 0.0001 && section.Header.ElementList.Any())
                        throw new InvalidOperationException("No height for the header has been set, but there are elements there.").AddData("ElementCount", section.Header.ElementList.Count);

                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Top, sectionBounds.Width, headerHeight);
                    var renderDataHeader = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debugData, includeBackgroundObjects, _documentProperties);
                    postRendering.Add(() => section.Header.Render(renderDataHeader, page));

                    if (_debugData != null)
                    {
                        gfx.DrawLine(_debugData.Pen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                    }
                }

                //Footer
                if (section.Footer != null)
                {
                    if (Math.Abs(footerHeight) < 0.0001 && section.Footer.ElementList.Any())
                        throw new InvalidOperationException("No height for the footer has been set, but there are elements there.").AddData("ElementCount", section.Footer.ElementList.Count);

                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Bottom - footerHeight, sectionBounds.Width, footerHeight);
                    var renderDataFooter = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debugData, includeBackgroundObjects, _documentProperties);
                    postRendering.Add(() => section.Footer.Render(renderDataFooter, page));

                    if (_debugData != null)
                    {
                        gfx.DrawLine(_debugData.Pen, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                    }
                }
            }

            foreach (var action in postRendering)
            {
                action();
            }
        }

        private int Max(params int[] items)
        {
            return items.Max();
        }

        private Section GetSection(bool preRender, int page)
        {
            var section = _template.SectionList.First();
            if (page > 0)
            {
                if (preRender)
                {
                    section = _template.SectionList.ToArray()[page];
                }
                else
                {
                    var tot = 0;
                    foreach (var s in _template.SectionList)
                    {
                        tot += s.GetRenderPageCount();
                        if (tot >= page + 1)
                        {
                            section = s;
                            section.SetPageOffset(tot - s.GetRenderPageCount());
                            break;
                        }
                    }
                }
            }

            return section;
        }

        private Document GetDocument(bool preRender, bool portrait)
        {
            var doc = new Document();
            if (!portrait)
                doc.DefaultPageSetup.Orientation = Orientation.Landscape;

            foreach (var section in _template.SectionList)
            {
                if (preRender)
                    doc.AddSection();
                else
                {
                    for (var i = 0; i < section.GetRenderPageCount(); i++)
                    {
                        doc.AddSection();
                    }
                }
            }

            return doc;
        }
    }
}