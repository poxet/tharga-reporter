using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class SinglePageAreaElement : AreaElement
    {
        internal abstract void Render(IRenderData renderData);
    }
}