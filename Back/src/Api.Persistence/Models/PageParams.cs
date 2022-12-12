namespace Api.Persistence.Models
{
    public class PageParams
    {
        public const int MaxPageSize = 100;
        public int PageNumber { get; set; } = 1;
        public int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }
        public string Term { get; set; } = string.Empty;
        public int? Categoria { get; set; } = null;
    }
}