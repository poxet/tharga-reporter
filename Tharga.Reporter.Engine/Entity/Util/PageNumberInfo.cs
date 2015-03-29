using System.Globalization;

namespace Tharga.Reporter.Engine.Entity.Util
{
    public class PageNumberInfo
    {
        private readonly int _pageNumber;
        private readonly int? _totalPages;

        public PageNumberInfo(int pageNumber, int? totalPages)
        {
            _pageNumber = pageNumber;
            _totalPages = totalPages;
        }

        public int PageNumber { get { return _pageNumber; } }
        public int? TotalPages { get { return _totalPages; } }

        //TODO: Write tests for this
        public string GetPageNumberInfo(string dataName)
        {
            switch (dataName)
            {
                case "PageNumber":
                    return PageNumber.ToString(CultureInfo.CurrentCulture);
                case "TotalPages":
                    return TotalPages == null ? "N/A" : TotalPages.Value.ToString(CultureInfo.CurrentCulture);
                default:
                    return null;
            }
        }
    }
}