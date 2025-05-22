namespace LaWash.IoT.Transversal;
public class BadResponseWithMessage : Exception
{
    public int StatusCode { get; }
    public string MessageStatus { get; }
    public BadResponseWithMessage(string message, int statusCode = 404)
        : base(message)
    {
        StatusCode = statusCode;
        MessageStatus = message;
    }
}

