namespace Resort_Hub.ViewModels.Home;

public class ErrorVM
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string? ExceptionMessage { get; set; }
    public string? ExceptionType { get; set; }
}
