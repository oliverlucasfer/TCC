namespace Api.Models
{
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemsPerPage, int totaItems, int totalPages)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totaItems;
            this.TotalPages = totalPages;
        }
        public int CurrentPage { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}