namespace DevCom.Application.DTOs.Common;

/// <summary>
/// Envelope genérico paginado.
/// Chave de cache: projects_list_p{page}_s{pageSize}_st{status}_cat{cat}_q{search}
/// </summary>
public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = new List<T>();
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }

    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, int page, int pageSize) =>
        new()
        {
            Items      = items,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            Page       = page,
            PageSize   = pageSize
        };
}
