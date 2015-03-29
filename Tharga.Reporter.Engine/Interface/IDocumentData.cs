using Tharga.Reporter.Engine.Entity;

namespace Tharga.Reporter.Engine.Interface
{
    public interface IDocumentData
    {
        string Get(string dataName);
        DocumentDataTable GetDataTable(string name);
    }
}