using Resort_Hub.Abstraction;

namespace Resort_Hub.Errors;

public static class VillaErrors
{
    public static readonly Error NotFound = 
        new("Villa not found", "Villa with the specified ID does not exist.",ErrorType.NotFound);

}
