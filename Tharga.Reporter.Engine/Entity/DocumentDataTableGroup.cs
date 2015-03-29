namespace Tharga.Reporter.Engine.Entity
{
    public class DocumentDataTableGroup : DocumentDataTableLine
    {
        private readonly string _content;

        public DocumentDataTableGroup(string content)
        {
            _content = content;
        }

        public string Content { get { return _content; } }
    }
}