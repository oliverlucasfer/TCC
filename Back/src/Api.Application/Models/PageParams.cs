namespace Api.Application.Models
{
    public class PageParams
    {
        public const int MaxPageSize = 100;
        private int _pageSize = 10;
        private int _pageNumber = 1;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string Term { get; set; } = string.Empty;
        public string Ano { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public int? Categoria { get; set; } = null;
    }
}