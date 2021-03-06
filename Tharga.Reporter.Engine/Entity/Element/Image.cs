using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class Image : SinglePageAreaElement
    {
        private string _source;
        private ECacheMode? _cacheMode;

        public enum ECacheMode
        {
            Preferably, //Download file and store locally if possible. Read file from cache if possible.
            Always, //Always download files locally. Read file from cache if possible.
            Never //Never download files locally. Never read from cache.
        }

        public string Source { get { return _source ?? string.Empty; } set { _source = value; } }
        public ECacheMode CacheMode { get { return _cacheMode ?? ECacheMode.Preferably; } set { _cacheMode = value; } }

        internal override void Render(IRenderData renderData)
        {
            if (IsNotVisible(renderData)) return;

            var bounds = GetBounds(renderData.ParentBounds);

            System.Drawing.Image imageData = null;
            try
            {
                imageData = GetImage(renderData.DocumentData, bounds);
                renderData.ElementBounds = GetImageBounds(imageData, bounds);

                if (renderData.IncludeBackground || !IsBackground)
                {
                    using (var image = XImage.FromGdiPlusImage(imageData))
                    {
                        renderData.Graphics.DrawImage(image, renderData.ElementBounds);
                    }
                }
            }
            catch (Exception e)
            {
                var f = new Font();
                var font = new XFont(f.GetName(renderData.Section), f.GetSize(renderData.Section) / 1.5, f.GetStyle(renderData.Section));
                var brush = new XSolidBrush(XColor.FromKnownColor(KnownColor.Transparent));
                renderData.Graphics.DrawRectangle(new XPen(f.GetColor(renderData.Section)), brush, bounds);
                var textBrush = new XSolidBrush(XColor.FromArgb(f.GetColor(renderData.Section)));

                try
                {
                    var nextTop = OutputText(renderData, e.Message, font, textBrush, new XPoint(bounds.Left, bounds.Top), bounds.Width);
                    if (e.Data.Contains("source"))
                        OutputText(renderData, e.Data["source"].ToString(), font, textBrush, new XPoint(bounds.Left, bounds.Top + nextTop), bounds.Width);
                }
                catch (Exception exception)
                {
                    renderData.Graphics.DrawString(exception.Message, font, brush, new XPoint(bounds.Left, bounds.Top), XStringFormats.TopLeft);
                }
            }
            finally
            {
                imageData?.Dispose();
            }
        }

        private static double OutputText(IRenderData renderData, string message, XFont font, XSolidBrush brush, XPoint point, double width)
        {
            var textSize = renderData.Graphics.MeasureString(message, font);
            var lineCount = 0;

            var offset = 0;
            var part = 0;
            var more = true;
            while (more) //offset + part < message.Length)
            {
                more = false;
                part = message.Length - offset;
                while (renderData.Graphics.MeasureString(message.Substring(offset, part), font).Width > width)
                {
                    part--;
                    more = true;
                }
                renderData.Graphics.DrawString(message.Substring(offset, part), font, brush, new XPoint(point.X,point.Y+ textSize.Height * lineCount), XStringFormats.TopLeft);

                offset = part;
                lineCount++;
            }


            return textSize.Height * lineCount;
        }

        private static XRect GetImageBounds(System.Drawing.Image imageData, XRect bounds)
        {
            var imageBounds = bounds;
            if (Math.Abs((imageBounds.Width / imageBounds.Height) - (imageData.Width / (double)imageData.Height)) > 0.01)
            {
                if (((imageBounds.Width / imageBounds.Height) - (imageData.Width / (double)imageData.Height)) > 0)
                    imageBounds.Width = (imageBounds.Height * imageData.Width) / imageData.Height;
                else
                    imageBounds.Height = (imageBounds.Width * imageData.Height) / imageData.Width;
            }

            return imageBounds;
        }

        public static string BytesToLongString(byte[] bytes)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < bytes.Length; i++)
                sb.AppendFormat("{0}/", bytes[i].ToString(CultureInfo.InvariantCulture));
            var imgData = sb.ToString();
            return imgData;
        }

        private static byte[] LongStringToBytes(string source)
        {
            var stringParts = source.Split('/');
            var bytes = new byte[stringParts.Length - 1];
            for (var i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)int.Parse(stringParts[i]);
            return bytes;
        }

        private System.Drawing.Image GetImage(IDocumentData documentData, XRect bounds)
        {
            var source = Source.ParseValue(documentData, null, null, false);

            try
            {
                System.Drawing.Image imageData;
                if (File.Exists(source))
                {
                    imageData = System.Drawing.Image.FromFile(source);
                }
                else if (WebResourceExists(source, out imageData))
                {
                }
                else if (!string.IsNullOrEmpty(source))
                {
                    using (var stream = new MemoryStream(LongStringToBytes(source)))
                    {
                        using (var image = System.Drawing.Image.FromStream(stream))
                        {
                            imageData = new Bitmap(image.Width, image.Height);
                            using (var gfx = Graphics.FromImage(imageData))
                            {
                                gfx.DrawImage(image, 0, 0, image.Width, image.Height);
                            }
                        }
                    }
                }

                if (imageData == null)
                {
                    throw new InvalidOperationException($"Cannot generate image data for source '{source}'.");
                }

                //if (imageData == null)
                //{
                //    imageData = new Bitmap((int)bounds.Width, (int)bounds.Height);
                //    using (var gfx = Graphics.FromImage(imageData))
                //    {
                //        var pen = new Pen(Color.Red);
                //        gfx.DrawLine(pen, 0, 0, imageData.Width, imageData.Height);
                //        gfx.DrawLine(pen, imageData.Width, 0, 0, imageData.Height);
                //        var font = new System.Drawing.Font("Verdana", 10);
                //        var brush = new SolidBrush(Color.DarkRed);
                //        gfx.DrawString(string.Format("Image '{0}' is missing.", source), font, brush, 0, 0);
                //    }
                //}

                return imageData;
            }
            catch (Exception exception)
            {
                exception.AddData("source", source);
                throw;
            }
        }

        private bool WebResourceExists(string image, out System.Drawing.Image imageData)
        {
            try
            {
                imageData = null;

                if (string.IsNullOrEmpty(image))
                    return false;

                Uri path;
                if (Uri.TryCreate(image, UriKind.Absolute, out path))
                {
                    if (CacheMode != ECacheMode.Never)
                    {
                        if (WebResourceByCache(image, ref imageData))
                        {
                            return true;
                        }
                    }

                    if (WebResourceDirectly(image, ref imageData))
                    {
                        return true;
                    }

                    return false;
                }
                else
                {
                    var encoding = Encoding.GetEncoding(1252);
                    var bytes = encoding.GetBytes(image);
                    using (var ms = new MemoryStream(bytes))
                    {
                        imageData = System.Drawing.Image.FromStream(ms);
                    }
                    return true;
                }
            }
            catch (Exception exception)
            {
                exception.AddData("image", image);
                throw;
            }
        }

        private static bool WebResourceDirectly(string image, ref System.Drawing.Image imageData)
        {
            using (var client = new WebClient())
            {
                using (var stream = client.OpenRead(image))
                {
                    if (stream != null)
                    {
                        imageData = System.Drawing.Image.FromStream(stream);
                        return true;
                    }
                    return false;
                }
            }
        }

        private bool WebResourceByCache(string image, ref System.Drawing.Image imageData)
        {
            var localName = image.Substring(image.IndexOf(":", StringComparison.Ordinal) + 3).Replace("/", "_").Replace("?", "_").Replace("=", "_").Replace("&", "_").Replace(":", "_");
            var cacheFileName = string.Format("{0}{1}", Path.GetTempPath(), localName);

            if (!File.Exists(cacheFileName))
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(image, cacheFileName);
                    }
                }
                catch (WebException exception)
                {
                    if (File.Exists(cacheFileName))
                    {
                        File.Delete(cacheFileName);
                    }

                    if (CacheMode == ECacheMode.Always)
                    {
                        throw new InvalidOperationException("Unable to download image file to file.", exception).AddData("cacheFileName", cacheFileName);
                    }

                    return false;
                }
            }

            try
            {
                imageData = System.Drawing.Image.FromFile(cacheFileName);
            }
            catch (OutOfMemoryException exception)
            {
                exception.AddData("cacheFileName", cacheFileName);
                throw;
            }

            return true;
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_source != null)
                xme.SetAttribute("Source", _source);

            if (_cacheMode != null)
                xme.SetAttribute("CacheMode", _cacheMode.ToString());

            return xme;
        }

        internal static Image Load(XmlElement xme)
        {
            var image = new Image();

            image.AppendData(xme);

            var xmlSource = xme.Attributes["Source"];
            if (xmlSource != null)
                image.Source = xmlSource.Value;

            if (xme.Attributes["CacheMode"] != null)
                image.CacheMode = (ECacheMode)Enum.Parse(typeof(ECacheMode), xme.Attributes["CacheMode"].Value);

            return image;
        }
    }
}