using System;
using System.Collections.Generic;
using System.Xml;
using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentData : IDocumentData
    {
        private readonly Dictionary<string, string> _data = new Dictionary<string, string>();
        private readonly List<DocumentDataTable> _dataTable = new List<DocumentDataTable>();

        public void Add(string key, string value)
        {
            _data.Add(key, value);
        }

        public string Get(string key)
        {
            return !_data.ContainsKey(key) ? null : _data[key];
        }

        public void Add(DocumentDataTable table)
        {
            var tbl = GetDataTable(table.TableName);
            if (tbl != null)
                throw new InvalidOperationException(string.Format("There is already a table named {0} in the document data.", tbl.TableName));
            _dataTable.Add(table);
        }

        public DocumentDataTable GetDataTable(string tableName)
        {
            var table = _dataTable.Find(itm => string.Compare(itm.TableName, tableName, StringComparison.InvariantCultureIgnoreCase) == 0);
            return table;
        }

        public XmlDocument ToXml()
        {
            var xmd = new XmlDocument();
            var xmeList = xmd.CreateElement("DocumentData");
            xmd.AppendChild(xmeList);

            var xmeItemList = xmd.CreateElement("Data");
            xmd.LastChild.AppendChild(xmeItemList);
            foreach (var item in _data)
            {
                var xme = xmd.CreateElement(item.Key);
                xme.InnerText = item.Value;
                xmeItemList.AppendChild(xme);
            }

            foreach (var table in _dataTable)
            {
                var xmeTable = xmd.CreateElement("Table");
                xmeTable.SetAttribute("Name", table.TableName);
                xmd.LastChild.AppendChild(xmeTable);
                foreach (var row in table.Rows)
                {
                    if (row is DocumentDataTableData)
                    {
                        var xmeRow = xmd.CreateElement("Row");
                        xmeTable.AppendChild(xmeRow);

                        var rowData = row as DocumentDataTableData;
                        foreach (var col in rowData.Columns)
                        {
                            var xmeCol = xmd.CreateElement(col.Key);
                            xmeCol.InnerText = col.Value;
                            xmeRow.AppendChild(xmeCol);
                        }
                    }
                    else if (row is DocumentDataTableGroup)
                    {
                        var xmeGroup = xmd.CreateElement("Group");
                        xmeTable.AppendChild(xmeGroup);

                        var rowData = row as DocumentDataTableGroup;
                        xmeGroup.InnerText = rowData.Content;
                    }
                    else
                        throw new ArgumentOutOfRangeException(string.Format("Unknown document data table type {0}.", row.GetType()));
                }
            }

            return xmd;
        }

        public static DocumentData Load(XmlDocument xmd)
        {
            var documentData = new DocumentData();

            var nodes = xmd.GetElementsByTagName("Data");
            foreach (XmlNode node in nodes[0])
            {
                documentData.Add(node.Name, node.InnerText);
            }

            var tables = xmd.GetElementsByTagName("Table");
            foreach (XmlElement table in tables)
            {
                var tableName = table.GetAttribute("Name");
                var t = new DocumentDataTable(tableName);
                documentData.Add(t);
                foreach (XmlElement row in table.ChildNodes)
                {
                    switch (row.Name)
                    {
                        case "Row":
                            var rw = new Dictionary<string, string>();
                            t.Rows.Add(new DocumentDataTableData(rw));
                            foreach (XmlElement col in row)
                            {
                                rw.Add(col.Name, col.InnerText);
                            }

                            break;
                        case "Group":
                            t.Rows.Add(new DocumentDataTableGroup(row.InnerText));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(string.Format("Unknown row type {0}.", row.Name));
                    }
                }
            }

            return documentData;
        }
    }
}