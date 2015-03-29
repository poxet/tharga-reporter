using System;

namespace Tharga.Reporter.Engine.Entity
{
    //TODO: Move stuff to extensions
    public struct UnitValue : IEquatable<UnitValue>
    {
        public enum EUnit
        {
            Point,
            Inch,
            Millimeter,
            Centimeter,
            Percentage
        }

        private readonly double _value;
        private readonly EUnit _unit;

        internal double Value { get { return _value; } }
        internal EUnit Unit { get { return _unit; } }

        internal UnitValue(double value, EUnit unit)
        {
            _value = value;
            _unit = unit;
        }

        private UnitValue(string s)
        {
            //Cut numbers to the left, from unit to the riht
            var regex = new System.Text.RegularExpressions.Regex("([-]|[.,]|[-.,]|[0-9])[0-9]*[.,]*[0-9]*");
            var collection = regex.Matches(s);

            if (collection.Count != 1) throw new InvalidOperationException(string.Format("Cannot parse {0}.", s));
            if (collection[0].Index != 0) throw new InvalidOperationException(string.Format("Cannot parse {0}.", s));

            //Get the value part
            var value = s.Substring(0, collection[0].Length);
            double d;
            value = value.Replace(".", System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            if (!double.TryParse(value, out d)) throw new InvalidOperationException(string.Format("Cannot parse {0} to a double.", value));
            _value = d;

            //Get the unit part
            _unit = s.Substring(collection[0].Length).ToUnit();
        }

        public static bool TryParse(string s, out UnitValue result)
        {
            result = new UnitValue(0, EUnit.Point);
            try
            {
                result = new UnitValue(s);
                return true;
            }
            catch (InvalidOperationException exp)
            {
                System.Diagnostics.Trace.TraceWarning(exp.Message);
                return false;
            }
        }

        public static UnitValue Parse(string s)
        {
            return new UnitValue(s);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is UnitValue && Equals((UnitValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_value.GetHashCode() * 397) ^ (int)_unit;
            }
        }

        public bool Equals(UnitValue other)
        {
            return _value.Equals(other._value) && _unit == other._unit;
        }

        public new string ToString()
        {
            return string.Format("{0}{1}", Value.ToString("0.####").Replace(System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, "."), Unit.ToShortString());
        }

        internal double GetXUnitValue(double totalValue)
        {
            PdfSharp.Drawing.XUnit value;
            switch (Unit)
            {
                case EUnit.Millimeter:
                    value = new PdfSharp.Drawing.XUnit(Value, PdfSharp.Drawing.XGraphicsUnit.Millimeter);
                    break;
                case EUnit.Centimeter:
                    value = new PdfSharp.Drawing.XUnit(Value, PdfSharp.Drawing.XGraphicsUnit.Centimeter);
                    break;
                case EUnit.Inch:
                    value = new PdfSharp.Drawing.XUnit(Value, PdfSharp.Drawing.XGraphicsUnit.Inch);
                    break;
                case EUnit.Percentage:
                    //Calculate the actual value, using provided total value
                    return Value / 100 * totalValue;
                case EUnit.Point:
                    return Value;
                default:
                    throw new InvalidOperationException(string.Format("Unknown unit {0}", Unit));
            }

            value.ConvertType(PdfSharp.Drawing.XGraphicsUnit.Point);
            return value.Value;
        }

        public static UnitValue operator -(UnitValue a, UnitValue b)
        {
            if (a.Unit == EUnit.Percentage && b.Unit != EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");
            if (a.Unit != EUnit.Percentage && b.Unit == EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");

            if (a.Unit == b.Unit)
                return new UnitValue(a.Value - b.Value, a.Unit);
            return new UnitValue(a.GetXUnitValue(0) - b.GetXUnitValue(0), EUnit.Point);
        }

        public static UnitValue operator +(UnitValue a, UnitValue b)
        {
            if (a.Unit == EUnit.Percentage && b.Unit != EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");
            if (a.Unit != EUnit.Percentage && b.Unit == EUnit.Percentage) throw new InvalidOperationException("Cannot use operators when the unit is in percentage, if not both values are in percentage.");

            if (a.Unit == b.Unit)
                return new UnitValue(a.Value + b.Value, a.Unit);
            return new UnitValue(a.GetXUnitValue(0) + b.GetXUnitValue(0), EUnit.Point);
        }

        public static bool operator ==(UnitValue a, UnitValue b)
        {
            if (((object)a) == ((object)b)) return true;
            if (((object)a) == null || ((object)b) == null) return false;
            return a.Equals(b);
        }

        public static bool operator !=(UnitValue a, UnitValue b)
        {
            if (((object)a) == ((object)b)) return false;
            if (((object)a) == null || ((object)b) == null) return true;
            return !a.Equals(b);
        }

        public static implicit operator string(UnitValue item)
        {
            return item.ToString();
        }

        public static implicit operator UnitValue(string item)
        {
            return Parse(item);
        }
    }
}