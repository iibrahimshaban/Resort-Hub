namespace Resort_Hub.Errors;

public static class UserError
{
    public static readonly Error NotFound = 
        new("User.NotFound", "The user you are lokking for was not found.", ErrorType.NotFound);

    public static readonly Error InvaildCredintial = 
        new("User.InvaildCredintial", "The username or password you entered is incorrect.", ErrorType.Unauthorized);

    public static readonly Error RegistrationFailed = 
        new("User.RegistrationFailed", "An error occurred while registering the user.", ErrorType.Failure);
    public static readonly Error ExternalLoginFailed =
        new("User.ExternalLoginFailed", "An error occurred while logging in with an external provider.", ErrorType.Failure);

    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Please confirm your email address before logging in.", ErrorType.Unauthorized);
    public static readonly Error AccountLockedOut = 
        new("User.AccountLockedOut", "Your account has been locked out due to multiple failed login attempts. Please try again later.", ErrorType.Unauthorized);
}
