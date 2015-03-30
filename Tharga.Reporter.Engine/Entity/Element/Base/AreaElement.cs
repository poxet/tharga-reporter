using System;
using System.Xml;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class AreaElement : Element
    {
        private readonly UnitRectangle _relativeAlignment;

        protected AreaElement()
        {
            _relativeAlignment = new UnitRectangle();
        }

        protected override XRect GetBounds(XRect parentBounds)
        {
            if (_relativeAlignment == null) throw new InvalidOperationException("No relative alignment for the Area.");
            var relativeAlignment = _relativeAlignment;

            var left = parentBounds.X + relativeAlignment.GetLeft(parentBounds.Width);
            var width = _relativeAlignment.GetWidht(parentBounds.Width);

            var top = parentBounds.Y + relativeAlignment.GetTop(parentBounds.Height);
            var height = relativeAlignment.GetHeight(parentBounds.Height);

            if (height < 0)
            {
                top = top + height;
                height = height * -1;
            }

            if (width < 0)
            {
                left = left + width;
                width = width * -1;
            }

            return new XRect(left, top, width, height);
        }

        protected override bool VerticalSwap(XRect parentBounds)
        {
            if (_relativeAlignment == null) throw new InvalidOperationException("No relative alignment for the Area.");
            var relativeAlignment = _relativeAlignment;

            var height = relativeAlignment.GetHeight(parentBounds.Height);
            return height < 0;
        }

        protected override bool HorixontalSwap(XRect parentBounds)
        {
            if (_relativeAlignment == null) throw new InvalidOperationException("No relative alignment for the Area.");
            var relativeAlignment = _relativeAlignment;

            var width = relativeAlignment.GetWidht(parentBounds.Width);
            return width < 0;
        }

        public override UnitValue? Top
        {
            get { return _relativeAlignment.Top; }
            set { _relativeAlignment.Top = value; }
        }

        public virtual UnitValue? Bottom
        {
            get { return _relativeAlignment.Bottom; }
            set { _relativeAlignment.Bottom = value; }
        }

        public virtual UnitValue? Height
        {
            get { return _relativeAlignment.Height; }
            set { _relativeAlignment.Height = value; }
        }

        public override UnitValue? Left
        {
            get { return _relativeAlignment.Left; }
            set { _relativeAlignment.Left = value; }
        }

        public virtual UnitValue? Right
        {
            get { return _relativeAlignment.Right; }
            set { _relativeAlignment.Right = value; }
        }

        public virtual UnitValue? Width
        {
            get { return _relativeAlignment.Width; }
            set { _relativeAlignment.Width = value; }
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (Left != null)
                xme.SetAttribute("Left", Left.Value.ToString());

            if (Top != null)
                xme.SetAttribute("Top", Top.Value.ToString());

            if (Right != null)
                xme.SetAttribute("Right", Right.Value.ToString());

            if (Bottom != null)
                xme.SetAttribute("Bottom", Bottom.Value.ToString());

            if (Width != null)
                xme.SetAttribute("Width", Width.Value.ToString());

            if (Height != null)
                xme.SetAttribute("Height", Height.Value.ToString());

            return xme;
        }

        protected override void AppendData(XmlElement xme)
        {
            base.AppendData(xme);
            Left = GetValue(xme, "Left");
            Top = GetValue(xme, "Top");
            Right = GetValue(xme, "Right");
            Bottom = GetValue(xme, "Bottom");
            Width = GetValue(xme, "Width");
            Height = GetValue(xme, "Height");
        }
    }
}