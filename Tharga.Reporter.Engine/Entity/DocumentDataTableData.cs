using System.Collections.Generic;

namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentDataTableData : DocumentDataTableLine
    {
        private readonly Dictionary<string, string> _columns;

        public DocumentDataTableData(Dictionary<string, string> columns)
        {
            _columns = columns;
        }

        public Dictionary<string, string> Columns { get { return _columns; } }
    }
}