namespace Resort_Hub.Errors;

public static class UserError
{
    public static readonly Error NotFound = 
        new("User.NotFound", "The user you are lokking for was not found.", ErrorType.NotFound);

    public static readonly Error InvaildCredintial = 
        new("User.InvaildCredintial", "The username or password you entered is incorrect.", ErrorType.Unauthorized);

    public static readonly Error RegistrationFailed = 
        new("User.RegistrationFailed", "An error occurred while registering the user.", ErrorType.Failure);
}
