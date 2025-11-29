namespace UniversityManagement.Shared.DTOs.Requests;

/// <summary>
/// Request for pagination parameters
/// </summary>
public class PaginationRequest
{
    private int _pageNumber = 1;
    private int _pageSize = 10;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => 10,
            > 100 => 100,
            _ => value
        };
    }

    /// <summary>
    /// Optional search query
    /// </summary>
    public string? SearchQuery { get; set; }

    /// <summary>
    /// Optional sort field
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Calculates skip value for database queries
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;
}
