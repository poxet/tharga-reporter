using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;
using Aspose.BarCode;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class BarCode : SinglePageAreaElement
    {
        private string _code;

        public string Code { get { return _code ?? string.Empty; } set { _code = value; } }

        internal override void Render(IRenderData renderData)
        {
            if (string.IsNullOrEmpty(Code))
                throw new InvalidOperationException("Code has not been set.");

            if (IsNotVisible(renderData)) return;

            var bounds = GetBounds(renderData.ParentBounds);

            renderData.ElementBounds = bounds;

            if (!IsBackground || renderData.IncludeBackground)
            {
                var b = new BarCodeBuilder { SymbologyType = Symbology.Code39Standard, CodeText = GetCode(renderData.DocumentData, renderData.PageNumberInfo) };
                var memStream = new MemoryStream();
                b.BarCodeImage.Save(memStream, ImageFormat.Png);
                var imageData = System.Drawing.Image.FromStream(memStream);

                //Paint over the license info
                using (var g = Graphics.FromImage(imageData))
                {
                    g.FillRectangle(new SolidBrush(b.BackColor), 0, 0, imageData.Width, 14);
                }

                using (var image = XImage.FromGdiPlusImage(imageData))
                {
                    renderData.Graphics.DrawImage(image, new XRect(renderData.ElementBounds.Left, renderData.ElementBounds.Top, renderData.ElementBounds.Width, renderData.ElementBounds.Height)); // - legendFontSize.Height));
                }

                imageData.Dispose();
            }
        }

        private string GetCode(IDocumentData documentData, PageNumberInfo pageNumberInfo)
        {
            return Code.ParseValue(documentData, pageNumberInfo);
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_code != null)
                xme.SetAttribute("Code", _code);

            return xme;
        }

        internal static BarCode Load(XmlElement xme)
        {
            var item = new BarCode();
            item.AppendData(xme);

            var xmlCode = xme.Attributes["Code"];
            if (xmlCode != null)
                item.Code = xmlCode.Value;

            return item;
        }
    }
}