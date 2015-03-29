using System;
using System.Collections.Generic;
using System.Linq;

namespace Tharga.Reporter.Engine.Entity.Area
{
    public class ElementList : List<Element.Element>
    {
        public T Get<T>(string elementName)
            where T : Element.Element
        {
            var item = this.SingleOrDefault(x => string.Compare(x.Name, elementName, StringComparison.InvariantCulture) == 0 && x.GetType() == typeof(T));
            return item as T;
        }
    }
}