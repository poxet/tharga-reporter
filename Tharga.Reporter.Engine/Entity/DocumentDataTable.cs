using System.Collections.Generic;

namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentDataTable
    {
        private readonly string _tableName;
        private readonly List<DocumentDataTableLine> _data = new List<DocumentDataTableLine>();

        public string TableName { get { return _tableName; } }
        public List<DocumentDataTableLine> Rows { get { return _data; } }

        public DocumentDataTable(string tableName)
        {
            _tableName = tableName;
        }

        public void AddRow(Dictionary<string, string> row)
        {
            _data.Add(new DocumentDataTableData(row));
        }

        public void AddGroup(string groupContent)
        {
            _data.Add(new DocumentDataTableGroup(groupContent));
        }
    }
}