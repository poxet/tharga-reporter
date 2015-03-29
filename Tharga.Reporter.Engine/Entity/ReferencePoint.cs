using System;
using System.Linq;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Area;
using Tharga.Reporter.Engine.Entity.Element;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity
{
    public class ReferencePoint : MultiPageElement
    {
        private const StackMethod _defaultStack = StackMethod.None;

        public enum StackMethod
        {
            None,
            Vertical
        }

        private ElementList _elementList;
        private StackMethod? _stack;
        private string _hideValue;

        public StackMethod Stack { get { return _stack ?? _defaultStack; } set { _stack = value; } }
        public ElementList ElementList { get { return _elementList ?? (_elementList = new ElementList()); } set { _elementList = value; } }
        public string HideValue { get { return _hideValue ?? string.Empty; } set { _hideValue = value; } }

        internal override int PreRender(IRenderData renderData)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = renderData.DocumentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return 0;
            }

            var bounds = GetBounds(renderData.ParentBounds);

            var rd = new RenderData(renderData.Graphics, bounds, renderData.Section, renderData.DocumentData, renderData.PageNumberInfo, renderData.DebugData, renderData.IncludeBackground);
            var pageCount = PreRenderChildren(rd);

            renderData.ElementBounds = new XRect(bounds.Left, bounds.Right, 0, 0);

            return pageCount;
        }

        internal override void Render(IRenderData renderData, int page)
        {
            if (!string.IsNullOrEmpty(HideValue))
            {
                var result = renderData.DocumentData.Get(HideValue);
                if (string.IsNullOrEmpty(result))
                    return;
            }

            var bounds = GetBounds(renderData.ParentBounds);

            var rd = new RenderData(renderData.Graphics, bounds, renderData.Section, renderData.DocumentData, renderData.PageNumberInfo, renderData.DebugData, renderData.IncludeBackground);
            RenderChildren(rd, page);

            if (renderData.DebugData != null)
            {
                const int radius = 10;
                renderData.Graphics.DrawEllipse(renderData.DebugData.Pen, bounds.Left - radius, bounds.Top - radius, radius * 2, radius * 2);
                renderData.Graphics.DrawLine(renderData.DebugData.Pen, bounds.Left - radius - 2, bounds.Top, bounds.Left + radius + 2, bounds.Top);
                renderData.Graphics.DrawLine(renderData.DebugData.Pen, bounds.Left, bounds.Top - radius - 2, bounds.Left, bounds.Top + radius + 2);
            }

            //TODO: Change the width and height to the actual area used
            renderData.ElementBounds = new XRect(bounds.Left, bounds.Right, 0, 0);
        }

        private void RenderChildren(IRenderData renderData, int page)
        {
            var stackTop = new UnitValue();
            foreach (var element in ElementList)
            {
                var resetLocation = false;
                if (Stack == StackMethod.Vertical)
                {
                    if (element.Top == null)
                    {
                        resetLocation = true;
                        element.Top = stackTop;
                    }
                }

                if (element is SinglePageAreaElement)
                    ((SinglePageAreaElement)element).Render(renderData);
                else if (element is MultiPageAreaElement)
                    ((MultiPageAreaElement)element).Render(renderData, page);
                else if (element is MultiPageElement)
                    ((MultiPageElement)element).Render(renderData, page);
                else
                    throw new ArgumentOutOfRangeException(string.Format("Unknown type {0}.", element.GetType()));

                stackTop = new UnitValue(stackTop.Value + renderData.ElementBounds.Height, stackTop.Unit);

                if (resetLocation)
                    element.Top = null;
            }
        }

        private int PreRenderChildren(IRenderData renderData)
        {
            var maxPageCount = 1;
            var elementsToRender = ElementList.Where(x => x is MultiPageAreaElement || x is MultiPageElement);

            var stackTop = new UnitValue();
            foreach (var element in elementsToRender)
            {
                var resetLocation = false;
                if (Stack == StackMethod.Vertical)
                {
                    if (element.Top == null)
                    {
                        resetLocation = true;
                        element.Top = stackTop;
                    }
                }

                int pageCount;
                if (element is MultiPageAreaElement)
                    pageCount = ((MultiPageAreaElement)element).PreRender(renderData);
                else if (element is MultiPageElement)
                    pageCount = ((MultiPageElement)element).PreRender(renderData);
                else
                    throw new ArgumentOutOfRangeException(string.Format("Unknown type {0}.", element.GetType()));

                stackTop = new UnitValue(stackTop.Value + renderData.ElementBounds.Height, stackTop.Unit);

                if (resetLocation)
                    element.Top = null;

                if (pageCount > maxPageCount)
                    maxPageCount = pageCount;
            }

            return maxPageCount;
        }

        protected override XRect GetBounds(XRect parentBounds)
        {
            var relativeAlignment = new UnitRectangle(Left.Value, Top.Value, "0", "0");

            var left = parentBounds.X + relativeAlignment.GetLeft(parentBounds.Width);
            var width = relativeAlignment.GetWidht(parentBounds.Width);

            var top = parentBounds.Y + relativeAlignment.GetTop(parentBounds.Height);
            var height = relativeAlignment.GetHeight(parentBounds.Height);

            if (height < 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("Height is adjusted from {0} to 0.", height));
                height = 0;
            }

            return new XRect(left, top, width, height);
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (Left != null)
                xme.SetAttribute("Left", Left.Value.ToString());

            if (Top != null)
                xme.SetAttribute("Top", Top.Value.ToString());

            if (_stack != null)
                xme.SetAttribute("Stack", _stack.ToString());

            if (_hideValue != null)
                xme.SetAttribute("HideValue", _hideValue);

            foreach (var element in ElementList)
            {
                var xmeElement = element.ToXme();
                var importedElement = xme.OwnerDocument.ImportNode(xmeElement, true);
                xme.AppendChild(importedElement);
            }

            return xme;
        }

        public static ReferencePoint Load(XmlElement xme)
        {
            var referencePoint = new ReferencePoint();
            referencePoint.AppendData(xme);

            referencePoint.Left = referencePoint.GetValue(xme, "Left");
            referencePoint.Top = referencePoint.GetValue(xme, "Top");

            var xmeStack = xme.Attributes["Stack"];
            if (xmeStack != null)
                referencePoint.Stack = (StackMethod)Enum.Parse(typeof(StackMethod), xmeStack.Value);

            var xmlHideValue = xme.Attributes["HideValue"];
            if (xmlHideValue != null)
                referencePoint.HideValue = xmlHideValue.Value;

            var elements = Pane.GetElements(xme);
            foreach (var element in elements)
                referencePoint.ElementList.Add(element);

            return referencePoint;
        }
    }
}