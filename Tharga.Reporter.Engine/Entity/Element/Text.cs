using System.Xml;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class Text : TextBase
    {
        private string _value;
        private string _hideValue;

        public string Value { get { return _value ?? string.Empty; } set { _value = value; } }
        public string HideValue { get { return _hideValue ?? string.Empty; } set { _hideValue = value; } }
        
        protected override string GetValue(IDocumentData documentData, PageNumberInfo pageNumberInfo)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = documentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return string.Empty;
            }

            return Value.ParseValue(documentData, pageNumberInfo);
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_hideValue != null)
                xme.SetAttribute("HideValue", _hideValue);

            if (_value != null)
                xme.SetAttribute("Value", _value);

            return xme;
        }

        internal static Text Load(XmlElement xme)
        {
            var text = new Text();

            text.AppendData(xme);

            var xmlHideValue = xme.Attributes["HideValue"];
            if (xmlHideValue != null)
                text.HideValue = xmlHideValue.Value;

            var xmlValue = xme.Attributes["Value"];
            if (xmlValue != null)
                text.Value = xmlValue.Value;

            return text;
        }
    }
}