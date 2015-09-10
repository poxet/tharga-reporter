using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml;
using PdfSharp.Drawing;
using Tharga.Reporter.Engine.Entity.Util;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public class Table : MultiPageAreaElement
    {
        private readonly Font _defaultContentFont = new Font();
        private readonly Font _defaultHeaderFont = new Font { Size = 18 };
        private readonly UnitValue _defaultRowPadding = "2px";
        private readonly UnitValue _defaultColumnPadding = "2px";

        public enum Alignment
        {
            Left,
            Right
        }

        public enum WidthMode
        {
            Specific,
            Auto,
            Spring
        }

        private readonly Dictionary<string, TableColumn> _columns = new Dictionary<string, TableColumn>();

        private Font _headerFont;
        private string _headerFontClass;
        private Color? _headerBorderColor;
        private Color? _headerBackgroundColor;

        private Font _contentFont;
        private string _contentFontClass;
        private Color? _contentBorderColor;
        private Color? _contentBackgroundColor;

        private Font _groupFont;
        private Color? _groupBorderColor;
        private Color? _groupBackgroundColor;
        private Color? _columnBorderColor;
        private UnitValue? _groupSpacing;

        private SkipLine _skipLine;

        private UnitValue? _rowPadding;
        private UnitValue? _columnPadding;

        internal Dictionary<string, TableColumn> Columns { get { return _columns; } }
        private List<PageRowSet> _pageRowSet;
        private string _hideTableWhenColumnIsHidden;

        public Font HeaderFont
        {
            get { return _headerFont ?? _defaultHeaderFont; }
            set
            {
                if (!string.IsNullOrEmpty(_headerFontClass))
                    throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFontClass has already been set.");
                _headerFont = value;
            }
        }

        internal string HeaderFontClass //TODO: Hidden because it is not yet fully implemented
        {
            get { return _headerFontClass ?? string.Empty; }
            set
            {
                if (_headerFont != null)
                    throw new InvalidOperationException("Cannot set both HeaderFont and HeaderFontClass. HeaderFont has already been set.");
                _headerFontClass = value;
            }
        }

        public Color? HeaderBorderColor { get { return _headerBorderColor; } set { _headerBorderColor = value; } }
        public Color? HeaderBackgroundColor { get { return _headerBackgroundColor; } set { _headerBackgroundColor = value; } }

        public Font ContentFont
        {
            get { return _contentFont ?? _defaultContentFont; }
            set
            {
                if (!string.IsNullOrEmpty(_contentFontClass))
                    throw new InvalidOperationException("Cannot set both ContentFont and ContentFontClass. ContentFontClass has already been set.");
                _contentFont = value;
            }
        }

        public UnitValue GroupSpacing { get { return _groupSpacing ?? new UnitValue(); } set { _groupSpacing = value; } }
        public Font GroupFont { get { return _groupFont ?? _contentFont ?? _defaultContentFont; } set { _groupFont = value; } }

        internal string ContentFontClass //TODO: Hidden because it is not yet fully implemented
        {
            get { return _contentFontClass ?? string.Empty; }
            set
            {
                if (_contentFont != null)
                    throw new InvalidOperationException("Cannot set both ContentFont and ContentFontClass. ContentFont has already been set.");
                _contentFontClass = value;
            }
        }

        public Color? ContentBorderColor { get { return _contentBorderColor; } set { _contentBorderColor = value; } }
        public Color? ContentBackgroundColor { get { return _contentBackgroundColor; } set { _contentBackgroundColor = value; } }
        public Color? GroupBorderColor { get { return _groupBorderColor; } set { _groupBorderColor = value; } }
        public Color? GroupBackgroundColor { get { return _groupBackgroundColor; } set { _groupBackgroundColor = value; } }
        public Color? ColumnBorderColor { get { return _columnBorderColor; } set { _columnBorderColor = value; } }
        public SkipLine SkipLine { get { return _skipLine; } set { _skipLine = value; } }
        public UnitValue RowPadding { get { return _rowPadding ?? _defaultRowPadding; } set { _rowPadding = value; } }
        public UnitValue ColumnPadding { get { return _columnPadding ?? _defaultColumnPadding; } set { _columnPadding = value; } }
        public string HideTableWhenColumnIsHidden { get { return _hideTableWhenColumnIsHidden; } set { _hideTableWhenColumnIsHidden = value; } }

        internal override int PreRender(IRenderData renderData)
        {
            _pageRowSet = new List<PageRowSet>();

            renderData.ElementBounds = GetBounds(renderData.ParentBounds);

            if (IsBackground && !renderData.IncludeBackground)
                return 0;

            var headerFont = new XFont(_headerFont.GetName(renderData.Section), _headerFont.GetSize(renderData.Section), _headerFont.GetStyle(renderData.Section));
            var lineFont = new XFont(_contentFont.GetName(renderData.Section), _contentFont.GetSize(renderData.Section), _contentFont.GetStyle(renderData.Section));
            var groupFont = new XFont(_groupFont.GetName(renderData.Section), _groupFont.GetSize(renderData.Section), _groupFont.GetStyle(renderData.Section));

            var firstColumn = _columns.FirstOrDefault();
            if (firstColumn.Value == null)
                return 0;

            var headerSize = renderData.Graphics.MeasureString(firstColumn.Value.Title, headerFont, XStringFormats.TopLeft);
            var stdLineSize = renderData.Graphics.MeasureString(firstColumn.Value.Title, lineFont, XStringFormats.TopLeft);

            if (renderData.DocumentData != null)
            {
                var dataTable = renderData.DocumentData.GetDataTable(Name);
                if (dataTable != null)
                {
                    var top = headerSize.Height + RowPadding.ToXUnit(renderData.ElementBounds.Height);
                    var pageIndex = 1;
                    var firstLineOnPage = 0;
                    for (var i = 0; i < dataTable.Rows.Count; i++)
                    {
                        var lineSize = stdLineSize;
                        if (dataTable.Rows[i] is DocumentDataTableData)
                        {
                            if (_skipLine != null && pageIndex % SkipLine.Interval == 0)
                                top += SkipLine.Height.ToXUnit(renderData.ElementBounds.Height);
                        }
                        else if (dataTable.Rows[i] is DocumentDataTableGroup)
                        {
                            top += GroupSpacing.ToXUnit(renderData.ElementBounds.Height);
                            lineSize = renderData.Graphics.MeasureString("X", groupFont, XStringFormats.TopLeft);
                            pageIndex = 0;
                        }

                        top += lineSize.Height;
                        top += RowPadding.ToXUnit(renderData.ElementBounds.Height);

                        pageIndex++;

                        if (top > renderData.ElementBounds.Height - lineSize.Height)
                        {
                            _pageRowSet.Add(new PageRowSet { FromRow = firstLineOnPage, ToRow = i });
                            firstLineOnPage = i + 1;
                            top = headerSize.Height + RowPadding.ToXUnit(renderData.ElementBounds.Height);
                        }
                    }

                    if (firstLineOnPage != dataTable.Rows.Count)
                        _pageRowSet.Add(new PageRowSet { FromRow = firstLineOnPage, ToRow = dataTable.Rows.Count - 1 });
                }
            }

            return _pageRowSet.Count;
        }

        internal override void Render(IRenderData renderData, int page)
        {
            if (IsNotVisible(renderData))
                return;

            var pushTextForwardOnPages = 0;
            if (Visibility == PageVisibility.LastPage && renderData.PageNumberInfo.TotalPages != page)
                pushTextForwardOnPages = renderData.PageNumberInfo.TotalPages.Value - 1;

            if (_pageRowSet == null) throw new InvalidOperationException("PreRendering has not yet been performed.");

            renderData.ElementBounds = GetBounds(renderData.ParentBounds);

            if (!renderData.IncludeBackground && IsBackground)
                return;

            var headerFont = new XFont(_headerFont.GetName(renderData.Section), _headerFont.GetSize(renderData.Section), _headerFont.GetStyle(renderData.Section));
            var headerBrush = new XSolidBrush(XColor.FromArgb(_headerFont.GetColor(renderData.Section)));
            var lineFont = new XFont(_contentFont.GetName(renderData.Section), _contentFont.GetSize(renderData.Section), _contentFont.GetStyle(renderData.Section));
            var lineBrush = new XSolidBrush(XColor.FromArgb(_contentFont.GetColor(renderData.Section)));
            var groupFont = new XFont(_groupFont.GetName(renderData.Section), _groupFont.GetSize(renderData.Section), _groupFont.GetStyle(renderData.Section));

            var firstColumn = _columns.FirstOrDefault();
            if (firstColumn.Value == null)
                return;
            var headerSize = renderData.Graphics.MeasureString(firstColumn.Value.Title, headerFont, XStringFormats.TopLeft);
            var stdLineSize = renderData.Graphics.MeasureString(firstColumn.Value.Title, lineFont, XStringFormats.TopLeft);

            if (renderData.DebugData != null)
                renderData.Graphics.DrawString(string.Format("Table: {0}", Name), renderData.DebugData.Font, renderData.DebugData.Brush, renderData.ElementBounds.Center);

            if (renderData.DocumentData != null)
            {
                var dataTable = renderData.DocumentData.GetDataTable(Name);
                if (dataTable != null)
                {
                    var columnPadding = ColumnPadding.ToXUnit(renderData.ElementBounds.Width);

                    foreach (var column in _columns.Where(x => x.Value.WidthMode == WidthMode.Auto).ToList())
                    {
                        //Get the size of the columnt title text
                        var stringSize = renderData.Graphics.MeasureString(column.Value.Title, headerFont, XStringFormats.TopLeft);
                        var wd = UnitValue.Parse((stringSize.Width + columnPadding).ToString(CultureInfo.InvariantCulture) + "px");

                        //If there is a fixed width value, start with that.
                        if (column.Value.Width == null)
                        {
                            column.Value.Width = wd;
                        }

                        //If the column header title is greater than
                        if (column.Value.Width.Value < wd)
                        {
                            column.Value.Width = wd;
                        }

                        foreach (var row in dataTable.Rows)
                        {
                            if (row is DocumentDataTableData)
                            {
                                var rowData = row as DocumentDataTableData;

                                var cellData = GetValue(column.Key, rowData.Columns);
                                stringSize = renderData.Graphics.MeasureString(cellData, lineFont, XStringFormats.TopLeft);
                                wd = UnitValue.Parse((stringSize.Width + columnPadding).ToString(CultureInfo.InvariantCulture) + "px");
                                if (column.Value.Width < wd)
                                {
                                    column.Value.Width = wd;
                                }
                            }
                        }
                    }

                    foreach (var column in _columns.ToList())
                    {
                        if (column.Value.HideValue != null)
                            column.Value.Hide = true;

                        foreach (var row in dataTable.Rows)
                        {
                            if (row is DocumentDataTableData)
                            {
                                var rowData = row as DocumentDataTableData;
                                var cellData = GetValue(column.Key, rowData.Columns);
                                var parsedHideValue = GetValue(column.Value.HideValue, rowData.Columns);
                                if (parsedHideValue != cellData)
                                    column.Value.Hide = false;
                            }
                        }

                        if (column.Value.Hide)
                        {
                            column.Value.Width = new UnitValue();
                            if (HideTableWhenColumnIsHidden == column.Key)
                            {
                                //Hide the entire table
                                return;
                            }
                        }
                    }

                    RenderBorder(renderData.ElementBounds, renderData.Graphics, headerSize);

                    var totalWidth = renderData.ElementBounds.Width;
                    var nonSpringWidth = _columns.Where(x => x.Value.WidthMode != WidthMode.Spring).Sum(x => x.Value.Width != null ? x.Value.Width.Value.ToXUnit(totalWidth) : 0);

                    var springCount = _columns.Count(x => x.Value.WidthMode == WidthMode.Spring && !x.Value.Hide);
                    if (springCount > 0)
                    {
                        foreach (var column in _columns.Where(x => x.Value.WidthMode == WidthMode.Spring && !x.Value.Hide).ToList())
                        {
                            if (column.Value.Width == null) column.Value.Width = "0";
                            var calculatedWidth = new UnitValue((renderData.ElementBounds.Width - nonSpringWidth) / springCount, UnitValue.EUnit.Point);
                            if (calculatedWidth > column.Value.Width) column.Value.Width = calculatedWidth;
                        }
                    }

                    AssureTotalColumnWidth(renderData.ElementBounds.Width);

                    //Create header
                    double left = 0;
                    var tableColumns = _columns.Values.Where(x => !x.Hide).ToList();
                    foreach (var column in tableColumns)
                    {
                        var alignmentJusttification = 0D;
                        if (column.Align == Alignment.Right)
                        {
                            var stringSize = renderData.Graphics.MeasureString(column.Title, headerFont, XStringFormats.TopLeft);
                            alignmentJusttification = column.Width.Value.ToXUnit(renderData.ElementBounds.Width) - stringSize.Width - (columnPadding / 2);
                        }
                        else
                        {
                            alignmentJusttification += columnPadding / 2;
                        }

                        renderData.Graphics.DrawString(column.Title, headerFont, headerBrush, new XPoint(renderData.ElementBounds.Left + left + alignmentJusttification, renderData.ElementBounds.Top), XStringFormats.TopLeft);
                        left += column.Width.Value.ToXUnit(renderData.ElementBounds.Width);

                        if (renderData.DebugData != null)
                            renderData.Graphics.DrawLine(renderData.DebugData.Pen, renderData.ElementBounds.Left + left, renderData.ElementBounds.Top, renderData.ElementBounds.Left + left, renderData.ElementBounds.Bottom);
                    }

                    var top = headerSize.Height + RowPadding.ToXUnit(renderData.ElementBounds.Height);
                    var pageIndex = 1;

                    var defaultRowset = true;
                    var pageRowSet = new PageRowSet { FromRow = 1 };
                    var index = page - renderData.Section.GetPageOffset() - pushTextForwardOnPages;
                    if (_pageRowSet.Count > index)
                    {
                        try
                        {
                            defaultRowset = false;
                            pageRowSet = _pageRowSet[index];
                        }
                        catch (Exception exception)
                        {
                            throw new InvalidOperationException(string.Format("_pageRowSet.Count={0}, index={1}", _pageRowSet.Count, index), exception);
                        }
                    }

                    //Draw column separator lines
                    if (ColumnBorderColor != null)
                    {
                        left = 0;
                        var borderPen = new XPen(ColumnBorderColor.Value, 0.1); //TODO: Set the thickness of the boarder
                        foreach (var column in _columns.Where(x => !x.Value.Hide).TakeAllButLast().ToList())
                        {
                            left += column.Value.Width.Value.ToXUnit(renderData.ElementBounds.Width);
                            renderData.Graphics.DrawLine(borderPen, renderData.ElementBounds.Left + left, renderData.ElementBounds.Top, renderData.ElementBounds.Left + left, renderData.ElementBounds.Bottom);
                        }
                    }

                    for (var i = pageRowSet.FromRow; i < pageRowSet.ToRow + 1; i++)
                    {
                        DocumentDataTableLine row;
                        try
                        {
                            row = dataTable.Rows[i];
                        }
                        catch (Exception exception)
                        {
                            var msg = string.Format("Looping from {0} to {1}. Currently at {2}, collection has {3} lines.", pageRowSet.FromRow, pageRowSet.ToRow + 1, i, dataTable.Rows.Count);
                            throw new InvalidOperationException(msg + string.Format("dataTable.Rows.Count={0}, i={1}, pageIndex={2}, page={3}, GetPageOffset={4}, index={5}, FromRow={6}, ToRow={7}, _pageRowSet.Count={8}, defaultRowset={9}", dataTable.Rows.Count, i, pageIndex, page, renderData.Section.GetPageOffset(), index, pageRowSet.FromRow, pageRowSet.ToRow, _pageRowSet.Count, defaultRowset), exception);
                        }

                        left = 0;
                        var lineSize = stdLineSize;
                        if (row is DocumentDataTableData)
                        {
                            var rowData = row as DocumentDataTableData;

                            foreach (var column in _columns.Where(x => !x.Value.Hide).ToList())
                            {
                                var cellData = GetValue(column.Key, rowData.Columns);

                                var alignmentJusttification = 0D;
                                if (column.Value.Align == Alignment.Right)
                                {                                    
                                    var stringSize = renderData.Graphics.MeasureString(cellData, lineFont, XStringFormats.TopLeft);
                                    alignmentJusttification = column.Value.Width.Value.ToXUnit(renderData.ElementBounds.Width) - stringSize.Width - (columnPadding / 2);
                                }
                                else
                                {
                                    alignmentJusttification += columnPadding / 2;
                                }

                                var parsedHideValue = GetValue(column.Value.HideValue, rowData.Columns);
                                if (parsedHideValue == cellData)
                                    cellData = string.Empty;

                                //TODO: If the string is too long. Cut the string down.
                                var calculatedCellData = AssureThatTextFitsInColumn(renderData, cellData, column, columnPadding, lineFont);

                                renderData.Graphics.DrawString(calculatedCellData, lineFont, lineBrush, new XPoint(renderData.ElementBounds.Left + left + alignmentJusttification, renderData.ElementBounds.Top + top), XStringFormats.TopLeft);
                                left += column.Value.Width.Value.ToXUnit(renderData.ElementBounds.Width);
                            }

                            if (_skipLine != null && pageIndex % SkipLine.Interval == 0)
                            {
                                var skipLineHeight = SkipLine.Height.ToXUnit(renderData.ElementBounds.Height);

                                if (SkipLine.BorderColor != null)
                                {
                                    renderData.Graphics.DrawLine(new XPen(XColor.FromArgb((Color)SkipLine.BorderColor), 0.1), renderData.ElementBounds.Left, renderData.ElementBounds.Top + top + lineSize.Height + skipLineHeight / 2, renderData.ElementBounds.Right, renderData.ElementBounds.Top + top + lineSize.Height + skipLineHeight / 2);
                                }

                                top += skipLineHeight;
                            }
                        }
                        else if (row is DocumentDataTableGroup)
                        {
                            var group = row as DocumentDataTableGroup;

                            if (pageIndex != 1)
                                top += GroupSpacing.ToXUnit(renderData.ElementBounds.Height);

                            var groupData = group.Content;
                            var stringSize = renderData.Graphics.MeasureString(groupData, groupFont, XStringFormats.TopLeft);
                            lineSize = stringSize;
                            var topLeftBox = new XPoint(renderData.ElementBounds.Left + left, renderData.ElementBounds.Top + top);
                            var topLeftText = new XPoint(renderData.ElementBounds.Left + left + (columnPadding / 2), renderData.ElementBounds.Top + top);

                            if (GroupBackgroundColor != null)
                            {
                                var brush = new XSolidBrush(XColor.FromArgb(GroupBackgroundColor.Value));
                                var rect = new XRect(topLeftBox, new XSize(renderData.ElementBounds.Width, stringSize.Height));
                                renderData.Graphics.DrawRectangle(new XPen(XColor.FromArgb(GroupBorderColor ?? GroupBackgroundColor.Value), 0.1), brush, rect);
                            }
                            else if (GroupBorderColor != null)
                            {
                                var rect = new XRect(topLeftBox, new XSize(renderData.ElementBounds.Width, stringSize.Height));
                                renderData.Graphics.DrawRectangle(new XPen(XColor.FromArgb(GroupBorderColor.Value), 0.1), rect);
                            }

                            renderData.Graphics.DrawString(groupData, groupFont, lineBrush, topLeftText, XStringFormats.TopLeft);
                            pageIndex = 0;
                        }

                        top += lineSize.Height;
                        top += RowPadding.ToXUnit(renderData.ElementBounds.Height);

                        pageIndex++;
                    }

                    ////Draw column separator lines
                    //if (ColumnBorderColor != null)
                    //{
                    //    left = 0;
                    //    var borderPen = new XPen(ColumnBorderColor.Value, 0.1); //TODO: Set the thickness of the boarder
                    //    foreach (var column in _columns.Where(x => !x.Value.Hide).TakeAllButLast().ToList())
                    //    {
                    //        left += column.Value.Width.Value.GetXUnitValue(renderData.ElementBounds.Width);
                    //        renderData.Graphics.DrawLine(borderPen, renderData.ElementBounds.Left + left, renderData.ElementBounds.Top, renderData.ElementBounds.Left + left, renderData.ElementBounds.Bottom);
                    //    }
                    //}
                }
            }

            if (renderData.DebugData != null)
                renderData.Graphics.DrawRectangle(renderData.DebugData.Pen, renderData.ElementBounds);
        }

        private void AssureTotalColumnWidth(double tableWidth)
        {
            var columnWidth = _columns.Sum(x => x.Value.Width != null ? x.Value.Width.Value.ToXUnit(tableWidth) : 0);
            if (columnWidth > tableWidth)
            {
                var totalColumnWidth = _columns.Where(x => !x.Value.Hide).Sum(x => (x.Value.Width ?? "0").ToXUnit(tableWidth));
                var decreaseProportion = tableWidth / totalColumnWidth;
                foreach (var column in _columns.Where(x => !x.Value.Hide).ToList())
                {
                    column.Value.Width = column.Value.Width * decreaseProportion;
                }
            }
        }

        private static string AssureThatTextFitsInColumn(IRenderData renderData, string cellData, KeyValuePair<string, TableColumn> column, double columnPadding, XFont lineFont)
        {
            if (string.IsNullOrEmpty(cellData)) return cellData;

            var columnWidth = column.Value.Width.Value.ToXUnit(renderData.ElementBounds.Width) - columnPadding;
            var originlTextWidth = renderData.Graphics.MeasureString(cellData, lineFont, XStringFormats.TopLeft).Width;
            if (columnWidth - originlTextWidth > -0.0001)
            {
                return cellData;
            }

            var calculatedCellData = cellData;
            XSize calculatedCellDataSize;
            do
            {
                calculatedCellData = calculatedCellData.Substring(0, calculatedCellData.Length - 1);
                calculatedCellDataSize = renderData.Graphics.MeasureString(calculatedCellData, lineFont, XStringFormats.TopLeft);
            } 
            while (calculatedCellDataSize.Width > columnWidth && calculatedCellData != string.Empty);

            return calculatedCellData;
        }

        private void RenderBorder(XRect elementBounds, IGraphics gfx, XSize headerSize)
        {
            if (ContentBorderColor != null || ContentBackgroundColor != null)
            {
                var borderPen = new XPen(ContentBorderColor ?? ContentBackgroundColor.Value, 0.1); //TODO: Set the thickness of the boarder

                if (ContentBackgroundColor != null)
                {
                    var brush = new XSolidBrush(XColor.FromArgb(ContentBackgroundColor.Value));
                    gfx.DrawRectangle(borderPen, brush, new XRect(elementBounds.Left, elementBounds.Top + headerSize.Height, elementBounds.Width, elementBounds.Height - headerSize.Height));
                }
                else
                    gfx.DrawRectangle(borderPen, new XRect(elementBounds.Left, elementBounds.Top + headerSize.Height, elementBounds.Width, elementBounds.Height - headerSize.Height));
            }

            if (HeaderBorderColor != null || HeaderBackgroundColor != null)
            {
                var borderPen = new XPen(HeaderBorderColor ?? HeaderBackgroundColor.Value, 0.1); //TODO: Se the thickness of the boarder

                if (HeaderBackgroundColor != null)
                {
                    var brush = new XSolidBrush(XColor.FromArgb(HeaderBackgroundColor.Value));
                    gfx.DrawRectangle(borderPen, brush, new XRect(elementBounds.Left, elementBounds.Top, elementBounds.Width, headerSize.Height));
                }
                else
                    gfx.DrawRectangle(borderPen, new XRect(elementBounds.Left, elementBounds.Top, elementBounds.Width, headerSize.Height));
            }
        }

        private string GetValue(string input, Dictionary<string, string> row)
        {
            var result = input.ParseValue(row);
            return result;
        }

        [Obsolete("Use function AddColumn that takes tableColumn as a parameter.")]
        public void AddColumn(string displayFormat, string displayName, UnitValue? width = null, WidthMode widthMode = WidthMode.Auto, Alignment alignment = Alignment.Left, string hideValue = null)
        {
            _columns.Add(displayFormat, new TableColumn { Value = displayFormat, Title = displayName, Width = width, Align = alignment, HideValue = hideValue, WidthMode = widthMode });
        }

        public void AddColumn(TableColumn tableColumn)
        {
            _columns.Add(tableColumn.Value, tableColumn);
        }

        internal override XmlElement ToXme()
        {
            var xme = base.ToXme();

            if (_contentBackgroundColor != null)
                xme.SetAttribute("ContentBackgroundColor", string.Format("{0}{1}{2}", _contentBackgroundColor.Value.R.ToString("X2"), _contentBackgroundColor.Value.G.ToString("X2"), _contentBackgroundColor.Value.B.ToString("X2")));

            if (_contentBorderColor != null)
                xme.SetAttribute("ContentBorderColor", string.Format("{0}{1}{2}", _contentBorderColor.Value.R.ToString("X2"), _contentBorderColor.Value.G.ToString("X2"), _contentBorderColor.Value.B.ToString("X2")));

            if (_groupBackgroundColor != null)
                xme.SetAttribute("GroupBackgroundColor", string.Format("{0}{1}{2}", _groupBackgroundColor.Value.R.ToString("X2"), _groupBackgroundColor.Value.G.ToString("X2"), _groupBackgroundColor.Value.B.ToString("X2")));

            if (_columnBorderColor != null)
                xme.SetAttribute("ColumnBorderColor", string.Format("{0}{1}{2}", _columnBorderColor.Value.R.ToString("X2"), _columnBorderColor.Value.G.ToString("X2"), _columnBorderColor.Value.B.ToString("X2")));

            if (_groupBorderColor != null)
                xme.SetAttribute("GroupBorderColor", string.Format("{0}{1}{2}", _groupBorderColor.Value.R.ToString("X2"), _groupBorderColor.Value.G.ToString("X2"), _groupBorderColor.Value.B.ToString("X2")));

            if (_headerBackgroundColor != null)
                xme.SetAttribute("HeaderBackgroundColor", string.Format("{0}{1}{2}", _headerBackgroundColor.Value.R.ToString("X2"), _headerBackgroundColor.Value.G.ToString("X2"), _headerBackgroundColor.Value.B.ToString("X2")));

            if (_headerBorderColor != null)
                xme.SetAttribute("HeaderBorderColor", string.Format("{0}{1}{2}", _headerBorderColor.Value.R.ToString("X2"), _headerBorderColor.Value.G.ToString("X2"), _headerBorderColor.Value.B.ToString("X2")));

            if (_headerFont != null)
            {
                var fontXme = _headerFont.ToXme("HeaderFont");
                if (xme.OwnerDocument == null) throw new NullReferenceException("There is no OwnerDocument for xme.");
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            if (_headerFontClass != null)
                xme.SetAttribute("HeaderFontClass", _headerFontClass);

            if (_contentFont != null)
            {
                var fontXme = _contentFont.ToXme("ContentFont");
                if (xme.OwnerDocument == null) throw new NullReferenceException("There is no OwnerDocument for xme.");
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            if (_contentFontClass != null)
                xme.SetAttribute("ContentFontClass", _contentFontClass);

            if (_groupFont != null)
            {
                var fontXme = _groupFont.ToXme("GroupFont");
                if (xme.OwnerDocument == null) throw new NullReferenceException("There is no OwnerDocument for xme.");
                var importedFont = xme.OwnerDocument.ImportNode(fontXme, true);
                xme.AppendChild(importedFont);
            }

            if (_skipLine != null)
            {
                var xmeSkipLine = _skipLine.ToXme();
                if (xme.OwnerDocument == null) throw new NullReferenceException("There is no OwnerDocument for xme.");
                var importeSkipLine = xme.OwnerDocument.ImportNode(xmeSkipLine, true);
                xme.AppendChild(importeSkipLine);
            }

            if (_rowPadding != null)
                xme.SetAttribute("RowPadding", _rowPadding.Value.ToString());

            if (_columnPadding != null)
                xme.SetAttribute("ColumnPadding", _columnPadding.Value.ToString());

            if (_hideTableWhenColumnIsHidden != null)
                xme.SetAttribute("HideTableWhenColumnIsHidden", _hideTableWhenColumnIsHidden);

            if (_groupSpacing != null)
                xme.SetAttribute("GroupSpacing", _groupSpacing.Value.ToString());

            var columns = xme.OwnerDocument.CreateElement("Columns");
            xme.AppendChild(columns);
            foreach (var column in Columns)
            {
                var xmeColumn = column.Value.ToXme();

                var col = columns.OwnerDocument.ImportNode(xmeColumn, true);
                columns.AppendChild(col);
            }

            return xme;
        }

        internal static Table Load(XmlElement xme)
        {
            var table = new Table();

            table.AppendData(xme);

            var xmlBackgroundColor = xme.Attributes["ContentBackgroundColor"];
            if (xmlBackgroundColor != null)
                table.ContentBackgroundColor = xmlBackgroundColor.Value.ToColor();

            var xmlBorderColor = xme.Attributes["ContentBorderColor"];
            if (xmlBorderColor != null)
                table.ContentBorderColor = xmlBorderColor.Value.ToColor();

            var xmlGroupBackgroundColor = xme.Attributes["GroupBackgroundColor"];
            if (xmlGroupBackgroundColor != null)
                table.GroupBackgroundColor = xmlGroupBackgroundColor.Value.ToColor();

            var xmlColumnBorderColor = xme.Attributes["ColumnBorderColor"];
            if (xmlColumnBorderColor != null)
                table.ColumnBorderColor = xmlColumnBorderColor.Value.ToColor();

            var xmlGroupBorderColor = xme.Attributes["GroupBorderColor"];
            if (xmlGroupBorderColor != null)
                table.GroupBorderColor = xmlGroupBorderColor.Value.ToColor();

            var xmlHeaderBackgroundColor = xme.Attributes["HeaderBackgroundColor"];
            if (xmlHeaderBackgroundColor != null)
                table.HeaderBackgroundColor = xmlHeaderBackgroundColor.Value.ToColor();

            var xmlHeaderBorderColor = xme.Attributes["HeaderBorderColor"];
            if (xmlHeaderBorderColor != null)
                table.HeaderBorderColor = xmlHeaderBorderColor.Value.ToColor();

            var xmlHeaderFontClass = xme.Attributes["HeaderFontClass"];
            if (xmlHeaderFontClass != null)
                table.HeaderFontClass = xmlHeaderFontClass.Value;

            var xmlLineFontClass = xme.Attributes["ContentFontClass"];
            if (xmlLineFontClass != null)
                table.ContentFontClass = xmlLineFontClass.Value;

            var xmlRowPadding = xme.Attributes["RowPadding"];
            if (xmlRowPadding != null)
                table.RowPadding = xmlRowPadding.Value;
            
            var xmlColumnPadding = xme.Attributes["ColumnPadding"];
            if (xmlColumnPadding != null)
                table.ColumnPadding = xmlColumnPadding.Value;

            var xmlHideTableWhenColumnIsHidden = xme.Attributes["HideTableWhenColumnIsHidden"];
            if (xmlHideTableWhenColumnIsHidden != null)
                table.HideTableWhenColumnIsHidden = xmlHideTableWhenColumnIsHidden.Value;

            var xmlGroupSpacing = xme.Attributes["GroupSpacing"];
            if (xmlGroupSpacing != null)
                table.GroupSpacing = xmlGroupSpacing.Value;

            foreach (XmlElement child in xme)
            {
                switch (child.Name)
                {
                    case "HeaderFont":
                        table.HeaderFont = Font.Load(child);
                        break;
                    case "ContentFont":
                        table.ContentFont = Font.Load(child);
                        break;
                    case "GroupFont":
                        table.GroupFont = Font.Load(child);
                        break;
                    case "SkipLine":
                        table.SkipLine = SkipLine.Load(child);
                        break;
                    case "Columns":
                        foreach (XmlElement xmlColumn in child.ChildNodes)
                        {
                            var col = TableColumn.Load(xmlColumn);
                            table.AddColumn(col);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(string.Format("Unknown subelement {0} to text base.", child.Name));
                }
            }

            return table;
        }
    }

    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> TakeAllButLast<T>(this IEnumerable<T> items)
        {
            T buffer = default(T);
            bool buffered = false;

            foreach (T x in items)
            {
                if (buffered)
                    yield return buffer;

                buffer = x;
                buffered = true;
            }
        }
    }
}