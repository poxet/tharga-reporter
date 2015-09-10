using System;
using System.Collections.Generic;
using Tharga.Reporter.Engine.Entity;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine
{
    internal static class Converter
    {
        public static string ToShortString(this UnitValue.EUnit unit)
        {
            switch (unit)
            {
                case UnitValue.EUnit.Point:
                    return string.Empty;
                case UnitValue.EUnit.Percentage:
                    return "%";
                case UnitValue.EUnit.Millimeter:
                    return "mm";
                case UnitValue.EUnit.Centimeter:
                    return "cm";
                case UnitValue.EUnit.Inch:
                    return "in";
                default:
                    throw new InvalidOperationException(string.Format("Cannot get short string for {0}", unit));
            }
        }

        public static UnitValue.EUnit ToUnit(this string unit)
        {
            switch (unit.ToLower().Trim())
            {
                case "": //Raw
                case "px":
                    return UnitValue.EUnit.Point;
                case "%": //Relative to parent area
                    return UnitValue.EUnit.Percentage;
                case "mm":
                    return UnitValue.EUnit.Millimeter;
                case "cm":
                    return UnitValue.EUnit.Centimeter;
                case "in":
                case "inch":
                    return UnitValue.EUnit.Inch;
                default:
                    throw new InvalidOperationException(string.Format("Cannot cast {0} to a unit.", unit));
            }
        }

        public static string ToShortTypeName(this Type type)
        {
            var fullName = type.ToString();
            var pos = fullName.LastIndexOf(".", StringComparison.Ordinal);
            return fullName.Substring(pos + 1);
        }

        public static string ParseValue(this string value, Dictionary<string, string> row)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            //Pick data fields and replace them with correct data
            var parsedValue = value;

            var startIndex = 0;
            var pos = parsedValue.IndexOf("{", startIndex, StringComparison.Ordinal);
            while (pos != -1)
            {
                var posE = parsedValue.IndexOf("}", pos, StringComparison.Ordinal);
                var dataName = parsedValue.Substring(pos + 1, posE - pos - 1);
                var dataValue = row.ContainsKey(dataName) ? row[dataName] : string.Format("[Data row '{0}' is missing]", dataName);
                startIndex = pos + dataValue.Length;
                parsedValue = string.Format("{0}{1}{2}", parsedValue.Substring(0, pos), dataValue, parsedValue.Substring(posE + 1));

                pos = parsedValue.IndexOf("{", startIndex, StringComparison.Ordinal);
            }

            return parsedValue.TrimEnd(' ');
        }

        public static string ParseValue(this string value, IDocumentData documentData, PageNumberInfo pageNumberInfo, DocumentProperties documentProperties, bool returnErrorMessage = true)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            //Pick data fields and replace them with correct data
            var parsedValue = value;

            var startIndex = 0;
            var pos = parsedValue.IndexOf("{", startIndex, StringComparison.Ordinal);
            while (pos != -1)
            {
                var posE = parsedValue.IndexOf("}", pos, StringComparison.Ordinal);
                var dataName = parsedValue.Substring(pos + 1, posE - pos - 1);

                string dataValue = null;
                if (pageNumberInfo != null)
                    dataValue = pageNumberInfo.GetPageNumberInfo(dataName);

                switch (dataName)
                {
                    case "Author":
                        dataValue = documentProperties.Author;
                        break;

                    case "Subject":
                        dataValue = documentProperties.Subject;
                        break;
                    
                    case "Title":
                        dataValue = documentProperties.Title;
                        break;

                    case "Creator":
                        dataValue = documentProperties.Creator;
                        break;
                }

                if (dataValue == null)
                    dataValue = (documentData != null && documentData.Get(dataName) != null) ? documentData.Get(dataName) : (returnErrorMessage ? string.Format("[Data '{0}' is missing]", dataName) : string.Empty);

                startIndex = pos + dataValue.Length;
                parsedValue = string.Format("{0}{1}{2}", parsedValue.Substring(0, pos), dataValue, parsedValue.Substring(posE + 1));

                pos = parsedValue.IndexOf("{", startIndex, StringComparison.Ordinal);
            }

            return parsedValue.TrimEnd(' ');
        }
    }
}