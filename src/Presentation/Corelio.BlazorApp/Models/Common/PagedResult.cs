namespace Corelio.BlazorApp.Models.Common;

/// <summary>
/// Generic paged result wrapper for list responses.
/// </summary>
/// <typeparam name="T">Type of the items in the list.</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// The list of items for the current page.
    /// </summary>
    public List<T> Items { get; set; } = [];

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of items across all pages.
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>
    /// Indicates if there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates if there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
