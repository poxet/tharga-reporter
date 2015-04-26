using System;
using PdfSharp;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Engine
{
    public class PageSizeInfo
    {
        private readonly PageSize? _pageSize;
        private readonly UnitValue? _width;
        private readonly UnitValue? _height;

        public PageSizeInfo(UnitValue width, UnitValue height)
        {
            _width = width;
            _height = height;
        }

        public PageSizeInfo(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "No name was provided.");

            PageSize pageSize;
            if (name.StartsWith("ISO", StringComparison.InvariantCulture))
            {
                name = name.Substring(3);
            }

            if (!Enum.TryParse(name, out pageSize))
            {
                throw new ArgumentOutOfRangeException(string.Format("Unable to parse '{0}' as page size.", name));
            }

            _pageSize = pageSize;
        }

        public PageSizeInfo(PageSize pageSize)
        {
            _pageSize = pageSize;
        }

        public bool IsCustomSize { get { return _pageSize == null; } }

        public PageSize PageSize
        {
            get
            {
                if (_pageSize == null) throw new InvalidOperationException("A custom size has been set. Use width and height properties.");
                return _pageSize.Value;
            }
        }

        public XUnit Width
        {
            get
            {
                if (_pageSize != null) throw new InvalidOperationException("A named document size has been set. Use PageSize property.");
                if (_width == null) throw new NullReferenceException("Width value is null.");
                return _width.Value.GetXUnitValue();
            }
        }

        public XUnit Height
        {
            get
            {
                if (_pageSize != null) throw new InvalidOperationException("A named document size has been set. Use PageSize property.");
                if (_height == null) throw new NullReferenceException("Height value is null.");
                return _height.Value.GetXUnitValue();
            }
        }
    }
}