using System;

namespace Tharga.Reporter.Engine
{
    internal static class PdfSharpExtensions
    {
        public static PdfSharp.PageSize ToPageSize(this Renderer.PageSize pageSize)
        {
            PdfSharp.PageSize result;
            if (!Enum.TryParse(pageSize.ToString(), true, out result))
                throw new InvalidOperationException(string.Format("Unable to parse page size {0} to PdfSharp version of page size.", pageSize));
            return result;
        }
    }
}