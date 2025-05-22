namespace LaWash.IoT.Transversal;

public class ErrorResponseWithMessage
{
    public int StatusCode { get; set; }
    public required string Message { get; set; }
}

