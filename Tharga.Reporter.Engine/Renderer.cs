﻿using System;
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

namespace Tharga.Reporter.Engine
{
    public class Renderer
    {
        public enum PageSize
        {
            A4,
            Letter
        }

        private readonly Template _template;
        private readonly DocumentData _documentData;
        private readonly bool _includeBackgroundObjects;
        private readonly DocumentProperties _documentProperties;
        private readonly IDebugData _debugData;
        private readonly IGraphicsFactory _graphicsFactory;
        private int _printPageCount;
        private bool _preRendered;

        internal Renderer(IGraphicsFactory graphicsFactory, Template template, DocumentData documentData = null, bool includeBackgroundObjects = true, DocumentProperties documentProperties = null, bool debug = false)
        {
            _template = template;
            _documentData = documentData;
            _includeBackgroundObjects = includeBackgroundObjects;
            _documentProperties = documentProperties;
            if (debug)
                _debugData = new DebugData();
            _graphicsFactory = graphicsFactory;
        }

        public Renderer(Template template, DocumentData documentData = null, bool includeBackgroundObjects = true, DocumentProperties documentProperties = null, bool debug = false)
            : this(new MyGraphicsFactory(), template, documentData, includeBackgroundObjects, documentProperties, debug)
        {
        }

        public void CreatePdfFile(string fileName, PageSize pageSize = PageSize.A4)
        {
            if (System.IO.File.Exists(fileName))
                throw new InvalidOperationException("The file already exists.").AddData("fileName", fileName);

            System.IO.File.WriteAllBytes(fileName, GetPdfBinary(pageSize));
        }

        public byte[] GetPdfBinary(PageSize pageSize = PageSize.A4)
        {
            PreRender(pageSize);

            var pdfDocument = CreatePdfDocument();
            RenderPdfDocument(pdfDocument, false, pageSize);

            var memStream = new System.IO.MemoryStream();
            pdfDocument.Save(memStream);
            return memStream.ToArray();
        }

        public void Print(PrinterSettings printerSettings)
        {
            _printPageCount = 0;

            PageSize pageSize;
            if (!Enum.TryParse(printerSettings.DefaultPageSettings.PaperSize.Kind.ToString(), out pageSize))
                throw new InvalidOperationException(string.Format("Unable to parse {0} from as printerSettings to a page size.", printerSettings.DefaultPageSettings.PaperSize.Kind));

            PreRender(pageSize);

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

        private void RenderPdfDocument(PdfDocument pdfDocument, bool preRender, PageSize pageSize)
        {
            if (_preRendered && preRender)
                throw new InvalidOperationException("Prerender has already been performed.");

            var doc = GetDocument(preRender);

            var docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();

            for (var ii = 0; ii < doc.Sections.Count; ii++)
            {
                var page = pdfDocument.AddPage();

                page.Size = pageSize.ToPageSize();

                var gfx = _graphicsFactory.PrepareGraphics(page, docRenderer, ii);

                DoRenderStuff(gfx, new XRect(0, 0, page.Width, page.Height), preRender, ii, _template.SectionList.Sum(x => x.GetRenderPageCount()));
            }

            if (preRender)
            {
                _preRendered = true;
            }
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

        private void PreRender(PageSize pageSize)
        {
            //TODO: If prerender with one format (pageSize) and printing with another.
            //or, if template or document data changed between render and pre-render then things will be messed up.
            if (!_preRendered)
            {
                var hasMultiPageElements = _template.SectionList.Any(x => x.Pane.ElementList.Any(y => y is MultiPageAreaElement || y is MultiPageElement));
                if (hasMultiPageElements)
                {
                    var pdfDocument = CreatePdfDocument();
                    RenderPdfDocument(pdfDocument, true, pageSize);
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

            DoRenderStuff(new MyGraphics(gfx), new XRect(unitSize), false, _printPageCount++, _template.SectionList.Sum(x => x.GetRenderPageCount()));
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

        private void DoRenderStuff(IGraphics gfx, XRect size, bool preRender, int page, int? totalPages)
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

            var renderData = new RenderData(gfx, paneBounds, section, _documentData, pageNumberInfo, _debugData, _includeBackgroundObjects);

            if (preRender)
            {
                var pageCount = section.Pane.PreRender(renderData);
                section.SetRenderPageCount(pageCount);
            }
            else
            {
                section.Pane.Render(renderData, page);

                //Header
                if (section.Header != null)
                {
                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Top, sectionBounds.Width, headerHeight);
                    var renderDataHeader = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debugData, _includeBackgroundObjects);
                    postRendering.Add(() => section.Header.Render(renderDataHeader, page));

                    if (_debugData != null)
                    {
                        gfx.DrawLine(_debugData.Pen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                    }
                }

                //Footer
                if (section.Footer != null)
                {
                    var bounds = new XRect(sectionBounds.Left, sectionBounds.Bottom - footerHeight, sectionBounds.Width, footerHeight);
                    var renderDataFooter = new RenderData(gfx, bounds, section, _documentData, pageNumberInfo, _debugData, _includeBackgroundObjects);
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