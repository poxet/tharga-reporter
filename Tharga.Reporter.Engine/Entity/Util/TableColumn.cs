using System;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Engine.Entity.Util
{
    public class TableColumn
    {
        private Table.WidthMode? _widthMode;
        private string _title;
        private Table.Alignment? _align;

        public TableColumn()
        {
            Hide = false;
        }

        public string Value { get; set; }
        public string Title { get { return _title ?? string.Empty; } set { _title = value; } }
        public UnitValue? Width { get; set; }
        public Table.WidthMode WidthMode { get { return _widthMode ?? Table.WidthMode.Auto; } set { _widthMode = value; } }
        public Table.Alignment Align { get { return _align ?? Table.Alignment.Left; } set { _align = value; } }
        public string HideValue { get; set; }

        internal bool Hide { get; set; }

        internal XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            if (Value != null)
                xme.SetAttribute("Value", Value);

            if (_title != null)
                xme.SetAttribute("Title", _title);

            if (_align != null)
                xme.SetAttribute("Align", _align.ToString());
            
            if (HideValue != null)
                xme.SetAttribute("HideValue", HideValue);

            if (Width != null)
                xme.SetAttribute("Width", Width.Value.ToString());

            if (_widthMode != null)
                xme.SetAttribute("WidthMode", _widthMode.ToString());

            return xme;
        }

        internal static TableColumn Load(XmlElement xme)
        {
            var tableColumn = new TableColumn();

            if (xme.Attributes["Value"] != null)
                tableColumn.Value = xme.Attributes["Value"].Value;

            //Just to support reading of old document types, the name of this property has changed to Value. First step is to throw when they are read. Then remove the property entierly.
            if (xme.Attributes["Key"] != null)
                tableColumn.Value = xme.Attributes["Key"].Value;

            if (string.IsNullOrEmpty(tableColumn.Value))
                throw new InvalidOperationException("The attribute value (or key) is missing in the template document.");

            if (xme.Attributes["Title"] != null) 
                tableColumn.Title = xme.Attributes["Title"].Value;

            //Just to support reading of old document types, the name of this property has changed to Title. First step is to throw when they are read. Then remove the property entierly.
            if (xme.Attributes["DisplayName"] != null)
                tableColumn.Title = xme.Attributes["DisplayName"].Value;

            if (xme.Attributes["Align"] != null) 
                tableColumn.Align = (Table.Alignment)Enum.Parse(typeof(Table.Alignment), xme.Attributes["Align"].Value);

            if (xme.Attributes["HideValue"] != null)
                tableColumn.HideValue = xme.Attributes["HideValue"].Value;
            
            if (xme.Attributes["Width"] != null)
                tableColumn.Width = UnitValue.Parse(xme.Attributes["Width"].Value);

            if (xme.Attributes["WidthMode"] != null)
                tableColumn.WidthMode = (Table.WidthMode)Enum.Parse(typeof(Table.WidthMode), xme.Attributes["WidthMode"].Value);

            return tableColumn;
        }
    }
}