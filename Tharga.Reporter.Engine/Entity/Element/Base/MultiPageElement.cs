using Tharga.Reporter.Engine.Interface;

namespace Tharga.Reporter.Engine.Entity.Element
{
    public abstract class MultiPageElement : Element
    {
        internal abstract int PreRender(IRenderData renderData);
        internal abstract void Render(IRenderData renderData, int page);
    }
}