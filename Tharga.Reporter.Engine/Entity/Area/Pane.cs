using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class Pane
    {
        private readonly ElementList _elementList = new ElementList();

        public ElementList ElementList
        {
            get { return _elementList; }
        }

        internal Pane()
        {
        }

        internal void Render(IRenderData renderData, int page)
        {
            foreach (var element in _elementList)
            {
                if (element as MultiPageElement != null)
                    ((MultiPageElement)element).Render(renderData, page);
                else if (element as MultiPageAreaElement != null)
                    ((MultiPageAreaElement)element).Render(renderData, page);
                else if (element as SinglePageAreaElement != null)
                    ((SinglePageAreaElement)element).Render(renderData);
                else
                    throw new ArgumentOutOfRangeException(string.Format("Unknown type {0} for pane to render.", element.GetType().Name));
            }
        }

        internal int PreRender(IRenderData renderData)
        {
            var maxTotalPages = 1;
            var elementsToRender = _elementList.Where(x => x is MultiPageAreaElement || x is MultiPageElement).ToArray();

            foreach (var element in elementsToRender)
            {
                var totalPages = 1;
                if (element as MultiPageElement != null)
                    totalPages = ((MultiPageElement)element).PreRender(renderData);
                else if (element as MultiPageAreaElement != null)
                    totalPages = ((MultiPageAreaElement)element).PreRender(renderData);
                else if (element as SinglePageAreaElement != null)
                    ((SinglePageAreaElement)element).Render(renderData);
                else
                    throw new ArgumentOutOfRangeException(string.Format("Unknown type {0} for pane to render.", element.GetType().Name));

                if (totalPages > maxTotalPages)
                    maxTotalPages = totalPages;
            }

            return maxTotalPages;
        }

        public virtual XmlElement ToXme()
        {
            var xmd = new XmlDocument();
            var header = xmd.CreateElement("Pane");

            var elms = GetElements(xmd);
            foreach (var elm in elms)
                header.AppendChild(elm);

            return header;
        }

        protected IEnumerable<XmlNode> GetElements(XmlDocument xmd)
        {
            foreach (var element in ElementList)
            {
                var xmeElement = element.ToXme();
                var importedElement = xmd.ImportNode(xmeElement, true);
                yield return importedElement;
            }
        }

        public static Pane Load(XmlElement xme)
        {
            var pane = new Pane();

            var elms = GetElements(xme);
            pane.ElementList.AddRange(elms);

            return pane;
        }

        internal static IEnumerable<Element.Element> GetElements(XmlElement xme)
        {
            foreach (XmlElement xmlElement in xme.ChildNodes)
            {
                Element.Element element;
                switch (xmlElement.Name)
                {
                    case "Image":
                        element = Image.Load(xmlElement);
                        break;
                    case "Line":
                        element = Line.Load(xmlElement);
                        break;
                    case "Rectangle":
                        element = Rectangle.Load(xmlElement);
                        break;
                    case "Table":
                        element = Table.Load(xmlElement);
                        break;
                    case "Text":
                        element = Text.Load(xmlElement);
                        break;
                    case "TextBox":
                        element = TextBox.Load(xmlElement);
                        break;
                    case "ReferencePoint":
                        element = ReferencePoint.Load(xmlElement);
                        break;
                    case "BarCode":
                        element = BarCode.Load(xmlElement);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Cannot parse element {0} as a subelement of pane.", xmlElement.Name));
                }

                yield return element;
            }
        }
    }
}