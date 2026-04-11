using Resort_Hub.Abstraction.Enums;

namespace Resort_Hub.Abstraction;

public record Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

}
