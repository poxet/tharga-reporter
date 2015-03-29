using System;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Element;

namespace Tharga.Reporter.Engine.Entity.Util
{
    internal class TableColumn
    {
        public string DisplayName { get; private set; }
        public UnitValue? Width { get; internal set; }
        public Table.WidthMode WidthMode { get; private set; }
        public Table.Alignment Align { get; private set; }
        public string HideValue { get; private set; }

        internal bool Hide { get; set; }

        //TODO: Have an empty default constructor here as well. And set values later on. As for all other classes.
        internal TableColumn(string displayName, UnitValue? width, Table.WidthMode widthMode, Table.Alignment align, string hideValue)
        {
            if (width == null && widthMode == Table.WidthMode.Specific) throw new InvalidOperationException("When not assigning a specific value for width the width mode cannot be set to specific.");

            DisplayName = displayName;
            Width = width;
            WidthMode = widthMode;
            Align = align;
            HideValue = hideValue;

            Hide = false;
        }

        internal XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var xme = xmd.CreateElement(GetType().ToShortTypeName());

            xme.SetAttribute("DisplayName", DisplayName);
            xme.SetAttribute("Align", Align.ToString());
            if (HideValue != null)
                xme.SetAttribute("HideValue", HideValue);
            if (Width != null)
                xme.SetAttribute("Width", Width.Value.ToString());
            xme.SetAttribute("WidthMode", WidthMode.ToString());

            return xme;
        }

        internal static TableColumn Load(XmlElement xme)
        {
            var displayName = xme.Attributes["DisplayName"].Value;
            var align = (Table.Alignment)Enum.Parse(typeof(Table.Alignment), xme.Attributes["Align"].Value);

            string hideValue = null;
            if (xme.Attributes["HideValue"] != null)
                hideValue = xme.Attributes["HideValue"].Value;

            UnitValue? width = null;
            if (xme.Attributes["Width"] != null)
                width = UnitValue.Parse(xme.Attributes["Width"].Value);

            var widthMode = (Table.WidthMode)Enum.Parse(typeof(Table.WidthMode), xme.Attributes["WidthMode"].Value);

            var tableColumn = new TableColumn(displayName, width, widthMode, align, hideValue);
            return tableColumn;
        }
    }
}