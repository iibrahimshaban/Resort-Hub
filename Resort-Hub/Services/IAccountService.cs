using Resort_Hub.ViewModels.Account;

namespace Resort_Hub.Services;

public interface IAccountService
{
    Task<Result<UserProfileVM>> GetProfileAsync(string UserId);
    Task<Result> UpdateProfileAsync(string UserId, UpdateProfileVM request);
    Task<Result> ChangePasswordAsync(string UserId, ChangePasswordVM request);
    Task<Result> UpdateProfilePictureAsync(string UserId, IFormFile Picture);
    Task<Result> DeleteProfilePictureAsync(string UserId);
}

