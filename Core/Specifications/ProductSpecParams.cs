namespace Core.Specifications
{
    public class ProductSpecParams
    {
        private const int MaxPageSize = 50;
        public int PageIndex { get; set; } = 1; // default value
        private int _pageSize = 6; // default value
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value; // if value is greater than MaxPageSize, then MaxPageSize, else value
        }
        public int? BrandId { get; set; } // ? means nullable
        public int? TypeId { get; set; } // ? means nullable
        public string Sort { get; set; } // sort by name, price, etc.
        private string _search;
        public string Search
        {
            get => _search;
            set => _search = value.ToLower(); // convert to lowercase
        }

    }
}