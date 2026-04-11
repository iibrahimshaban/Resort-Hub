using Resort_Hub.Abstraction;

namespace Resort_Hub.Errors;

public static class TestError
{
    public static readonly Error NotFound = new("NotFound", "The requested resource was not found.", ErrorType.NotFound);
}
