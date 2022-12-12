using System.Text.Json;
using Api.Models;
using Microsoft.AspNetCore.Http;

namespace Api.Extensions
{
    public static class Pagination
    {
        public static void AddPagination(this HttpResponse response, int currentPage, int itemsPerPage, int totaItems, int totalPages)
        {
            var pagination = new PaginationHeader(currentPage, itemsPerPage, totaItems, totalPages);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(pagination, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}