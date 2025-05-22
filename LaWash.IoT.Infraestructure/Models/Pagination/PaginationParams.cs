namespace LaWash.IoT.Infraestructure;

public class PaginationParams
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public int Skip => (PageNumber - 1) * PageSize;
}

