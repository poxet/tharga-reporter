using System;
using System.Xml;
using PdfSharp.Drawing;

namespace Tharga.Reporter.Engine.Entity
{
    public class UnitRectangle : IEquatable<UnitRectangle>
    {
        private UnitValue? _left;
        private UnitValue? _top;
        private UnitValue? _right;
        private UnitValue? _bottom;
        private UnitValue? _width;
        private UnitValue? _height;

        public UnitRectangle()
        {
        }

        internal UnitRectangle(UnitValue left, UnitValue top, UnitValue right, UnitValue bottom)
        {
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }

        public UnitValue? Left
        {
            get { return _left; }

            set
            {
                if (_right != null && _width != null && value != null) throw new InvalidOperationException(string.Format("Only two of the values left, right and width can be set, the third one is calculated. Right ({0}) and width {1} has already been set.", _right.ToString(), _width.ToString()));
                _left = value;
            }
        }

        public UnitValue? Top
        {
            get { return _top; }

            set
            {
                if (_bottom != null && _height != null && value != null) throw new InvalidOperationException(string.Format("Only two of the values top, bottom and height can be set, the third one is calculated. Bottom ({0}) and height {1} has already been set.", _bottom.ToString(), _height.ToString()));
                _top = value;
            }
        }

        public UnitValue? Right
        {
            get { return _right; }
            set
            {
                if (_left != null && _width != null && value != null) throw new InvalidOperationException(string.Format("Only two of the values left, right and width can be set, the third one is calculated. Left ({0}) and width {1} has already been set.", _left.ToString(), _width.ToString()));
                _right = value;
            }
        }

        public UnitValue? Bottom
        {
            get { return _bottom; }
            set
            {
                if (_top != null && _height != null && value != null) throw new InvalidOperationException(string.Format("Only two of the values top, bottom and height can be set, the third one is calculated. Top ({0}) and height {1} has already been set.", _top.ToString(), _height.ToString()));
                _bottom = value;
            }
        }

        public UnitValue? Width
        {
            get { return _width; }
            set
            {
                if (_left != null && _right != null && value != null) throw new InvalidOperationException(string.Format("Only two of the values left, right and width can be set, the third one is calculated. Left ({0}) and right {1} has already been set.", _left.ToString(), _right.ToString()));
                _width = value;
            }
        }

        public UnitValue? Height
        {
            get { return _height; }
            set
            {
                if (_top != null && _bottom != null && value != null) throw new InvalidOperationException(string.Format("Only two of the values top, bottom and height can be set, the third one is calculated. Top ({0}) and bottom {1} has already been set.", _top.ToString(), _bottom.ToString()));
                _height = value;
            }
        }

        internal double GetLeft(double totalWidth)
        {
            if (_left != null) return _left.Value.ToXUnit(totalWidth);

            if (_right != null && _width != null) return totalWidth - _width.Value.ToXUnit(totalWidth) - _right.Value.ToXUnit(totalWidth);

            return 0;
        }

        internal double GetTop(double totalHeight)
        {
            if (_top != null) return _top.Value.ToXUnit(totalHeight);

            if (_bottom != null && _height != null) throw new NotImplementedException();

            return 0;
        }

        internal double GetRight(double totalWidth)
        {
            if (_right != null) return _right.Value.ToXUnit(totalWidth);

            if (_left != null && _width != null) return totalWidth - (_left.Value.ToXUnit(totalWidth) + _width.Value.ToXUnit(totalWidth));

            return 0;
        }

        internal double GetBottom(double totalHeight)
        {
            if (_bottom != null) return _bottom.Value.ToXUnit(totalHeight);

            if (_top != null && _height != null) return totalHeight - (_top.Value.ToXUnit(totalHeight) + _height.Value.ToXUnit(totalHeight));

            return 0;
        }

        internal double GetWidht(double totalWidth)
        {
            if (_width != null) return _width.Value.ToXUnit(totalWidth);

            if (_left != null && _right != null) return totalWidth - _left.Value.ToXUnit(totalWidth) - _right.Value.ToXUnit(totalWidth);

            if (_left != null && _right == null) return totalWidth - _left.Value.ToXUnit(totalWidth);

            if (_left == null && _right != null) return totalWidth - _right.Value.ToXUnit(totalWidth);

            return totalWidth;
        }

        internal double GetHeight(double totalHeight)
        {
            if (_height != null) return _height.Value.ToXUnit(totalHeight);

            if (_top != null && _bottom != null) return totalHeight - _top.Value.ToXUnit(totalHeight) - _bottom.Value.ToXUnit(totalHeight);

            if (_top != null) return totalHeight - _top.Value.ToXUnit(totalHeight);

            if (_bottom != null) return totalHeight - _bottom.Value.ToXUnit(totalHeight);

            return totalHeight;
        }

        public static bool operator ==(UnitRectangle a, UnitRectangle b)
        {
            if (((object)a) == ((object)b)) return true;
            if (((object)a) == null || ((object)b) == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(UnitRectangle a, UnitRectangle b)
        {
            if (((object)a) == ((object)b)) return false;
            if (((object)a) == null || ((object)b) == null) return true;
            return !a.Equals(b);
        }

        public bool Equals(UnitRectangle other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _left.Equals(other._left) && _top.Equals(other._top) && _right.Equals(other._right) && _bottom.Equals(other._bottom) && _width.Equals(other._width) && _height.Equals(other._height);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UnitRectangle)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _left.GetHashCode();
                hashCode = (hashCode * 397) ^ _top.GetHashCode();
                hashCode = (hashCode * 397) ^ _right.GetHashCode();
                hashCode = (hashCode * 397) ^ _bottom.GetHashCode();
                hashCode = (hashCode * 397) ^ _width.GetHashCode();
                hashCode = (hashCode * 397) ^ _height.GetHashCode();
                return hashCode;
            }
        }

        internal XmlElement ToXme(string elementName = "Rect")
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(elementName);

            if (Left != null) xme.SetAttribute("Left", Left.Value.ToString());

            if (Top != null) xme.SetAttribute("Top", Top.Value.ToString());

            if (Right != null) xme.SetAttribute("Right", Right.Value.ToString());

            if (Bottom != null) xme.SetAttribute("Bottom", Bottom.Value.ToString());

            if (Width != null) xme.SetAttribute("Width", Width.Value.ToString());

            if (Height != null) xme.SetAttribute("Height", Height.Value.ToString());

            return xme;
        }

        internal static UnitRectangle Load(XmlElement xme)
        {
            var rect = new UnitRectangle();

            var left = xme.Attributes["Left"];
            if (left != null) rect.Left = UnitValue.Parse(left.Value);

            var top = xme.Attributes["Top"];
            if (top != null) rect.Top = UnitValue.Parse(top.Value);

            var right = xme.Attributes["Right"];
            if (right != null) rect.Right = UnitValue.Parse(right.Value);

            var bottom = xme.Attributes["Bottom"];
            if (bottom != null) rect.Bottom = UnitValue.Parse(bottom.Value);

            var width = xme.Attributes["Width"];
            if (width != null) rect.Width = UnitValue.Parse(width.Value);

            var height = xme.Attributes["Height"];
            if (height != null) rect.Height = UnitValue.Parse(height.Value);

            return rect;
        }

        internal XRect ToXRect(XGraphicsUnit type = XGraphicsUnit.Point)
        {
            if (Width != null && Height != null)
            {
                return new XRect(Left.Value.ToXUnit(type), Top.Value.ToXUnit(type), Width.Value.ToXUnit(type), Height.Value.ToXUnit(type));
            }
            else if (Right != null && Bottom != null)
            {
                return new XRect(Left.Value.ToXUnit(type), Top.Value.ToXUnit(type), Right.Value.ToXUnit(type), Bottom.Value.ToXUnit(type));
            }
            else
            {
                throw new InvalidOperationException("Cannot convert to rect size Width and Height is null and Right and Bottom is null.");
            }
        }
    }
}