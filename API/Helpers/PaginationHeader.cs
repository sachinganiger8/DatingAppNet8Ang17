using System;

namespace API.Helpers;

public class PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
{
    public int CurrentPage { get; set; } = currentPage;
    public int itemsPerPage { get; set; } = itemsPerPage;
    public int totalItems { get; set; } = totalItems;
    public int TotalPages { get; set; } = totalPages;
}
