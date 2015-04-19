using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using MigraDoc.Rendering.Printing;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;
using Section = Tharga.Reporter.Engine.Entity.Section;
using PdfSharp;

namespace Tharga.Reporter.Engine
{
    public class Renderer
    {
        private readonly Template _template;
        private readonly DocumentData _documentData;
        private readonly DocumentProperties _documentProperties;
        private readonly IDebugData _debugData;
        private readonly IGraphicsFactory _graphicsFactory;
        private readonly PageSizeInfo _pageSizeInfo;
        private int _printPageCount;
        private bool _preRendered;
        private bool _includeBackgroundObjects = true;

        internal Renderer(IGraphicsFactory graphicsFactory, Template template, DocumentData documentData = null, DocumentProperties documentProperties = null, PageSizeInfo pageSizeInfo = null, bool debug = false)
        {
            _template = template;
            _documentData = documentData;
            _documentProperties = documentProperties;
            _pageSizeInfo = pageSizeInfo ?? new PageSizeInfo(PageSize.A4);
            if (debug)
                _debugData = new DebugData();
            _graphicsFactory = graphicsFactory;
        }

        public Renderer(Template template, DocumentData documentData = null, DocumentProperties documentProperties = null, PageSizeInfo pageSizeInfo = null, bool debug = false)
            : this(new MyGraphicsFactory(), template, documentData, documentProperties, pageSizeInfo, debug)
        {
        }

        public void CreatePdfFile(string fileName, bool includeBackgroundObjects = true)
        {
            if (System.IO.File.Exists(fileName))
                throw new InvalidOperationException("The file already exists.").AddData("fileName", fileName);

            System.IO.File.WriteAllBytes(fileName, GetPdfBinary(includeBackgroundObjects));
        }

        public byte[] GetPdfBinary(bool includeBackgroundObjects = true)
        {
            PreRender(_pageSizeInfo, includeBackgroundObjects);

            var pdfDocument = CreatePdfDocument();
            RenderPdfDocument(pdfDocument, false, _pageSizeInfo, includeBackgroundObjects);

            var memStream = new System.IO.MemoryStream();
            pdfDocument.Save(memStream);
            return memStream.ToArray();
        }

        public void Print(PrinterSettings printerSettings, bool includeBackgroundObjects = true)
        {
            _printPageCount = 0;
            _includeBackgroundObjects = includeBackgroundObjects;

            var pageSizeInfo = new PageSizeInfo(printerSettings.DefaultPageSettings.PaperSize.Kind.ToString());

            PreRender(pageSizeInfo, includeBackgroundObjects);

            var doc = GetDocument(false);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            var printDocument = new MigraDocPrintDocument();
            printDocument.PrintController = new StandardPrintController();

            printDocument.PrintPage += PrintDocument_PrintPage;

            printDocument.Renderer = docRenderer;
            printDocument.PrinterSettings = printerSettings;
            printDocument.Print();
        }

        private void RenderPdfDocument(PdfDocument pdfDocument, bool preRender, PageSizeInfo pageSizeInfo, bool includeBackgroundObjects)
        {
            if (_preRendered && preRender)
                throw new InvalidOperationException("Prerender has already been performed.");

            var doc = GetDocument(preRender);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            for (var ii = 0; ii < doc.Sections.Count; ii++)
            {
                var page = AddPage(pdfDocument, pageSizeInfo);

                var gfx = _graphicsFactory.PrepareGraphics(page, docRenderer, ii);

                DoRenderStuff(gfx, new XRect(0, 0, page.Width, page.Height), preRender, ii, _template.SectionList.Sum(x => x.GetRenderPageCount()), includeBackgroundObjects);
            }

            if (preRender)
            {
                _preRendered = true;
            }
        }

        private static PdfPage AddPage(PdfDocument pdfDocument, PageSizeInfo pageSizeInfo)
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
            pdfDocument.SecuritySettings.DocumentSecurityLevel = PdfSharp.Pdf.Security.PdfDocumentSecurityLevel.Encrypted128Bit;
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

        private void PreRender(PageSizeInfo pageSizeInfo, bool includeBackgroundObjects)
        {
            //TODO: If prerender with one format (pageSize) and printing with another.
            //or, if template or document data changed between render and pre-render then things will be messed up.
            if (!_preRendered)
            {
                var hasMultiPageElements = _template.SectionList.Any(x => x.Pane.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement) || x.Header.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement) || x.Footer.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement));
                if (hasMultiPageElements)
                {
                    var pdfDocument = CreatePdfDocument();
                    RenderPdfDocument(pdfDocument, true, pageSizeInfo, includeBackgroundObjects);
                }
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            var rawSize = GetSize(e);
            var unitSize = GetSize(rawSize);
            var scale = GetScale(unitSize);

            var gfx = XGraphics.FromGraphics(e.Graphics, rawSize, XGraphicsUnit.Point);
            gfx.ScaleTransform(scale);

            DoRenderStuff(new MyGraphics(gfx), new XRect(unitSize), false, _printPageCount++, _template.SectionList.Sum(x => x.GetRenderPageCount()), _includeBackgroundObjects);
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
            var w = e.PageBounds.Width;
            var h = e.PageBounds.Height;
            var size = new XSize(w, h);
            return size;
        }

        private void DoRenderStuff(IGraphics gfx, XRect size, bool preRender, int page, int? totalPages, bool includeBackgroundObjects)
        {
            var postRendering = new List<Action>();

            var pageNumberInfo = new PageNumberInfo(page + 1, totalPages);

            var section = GetSection(preRender, page);

            var sectionBounds = new XRect(section.Margin.GetLeft(size.Width), section.Margin.GetTop(size.Height), section.Margin.GetWidht(size.Width), section.Margin.GetHeight(size.Height));

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

            var headerHeight = section.Header.Height.GetXUnitValue(sectionBounds.Height);
            var footerHeight = section.Footer.Height.GetXUnitValue(sectionBounds.Height);
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

        private Document GetDocument(bool preRender)
        {
            var doc = new Document();

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