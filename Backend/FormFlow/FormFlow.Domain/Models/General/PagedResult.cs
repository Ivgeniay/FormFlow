namespace FormFlow.Domain.Models.General
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public PaginationInfo Pagination { get; set; } = new PaginationInfo();

        public PagedResult() { }

        public PagedResult(List<T> data, int totalCount, int page, int pageSize)
        {
            Data = data;
            Pagination = new PaginationInfo
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                HasPrevious = page > 1,
                HasNext = page < (int)Math.Ceiling((double)totalCount / pageSize)
            };
        }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
