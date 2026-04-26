using CloudinaryDotNet;
using Resort_Hub.ViewModels.Account;

namespace Resort_Hub.Services;

public class AccountService(UserManager<ApplicationUser> userManager, ICloudinaryService cloudinaryService) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ICloudinaryService _cloudinaryService = cloudinaryService;

    public async Task<Result<UserProfileVM>> GetProfileAsync(string UserId)
    {
        var profile = await _userManager.Users
            .Where(x => x.Id == UserId)
            .ProjectToType<UserProfileVM>()
            .SingleAsync();

        return Result.Success(profile);
    }
    public async Task<Result> UpdateProfileAsync(string UserId, UpdateProfileVM request)
    {
        await _userManager.Users
             .Where(x => x.Id == UserId)
             .ExecuteUpdateAsync(setters =>
                  setters
                     .SetProperty(u => u.UserName, request.UserName)
                     .SetProperty(u => u.FirstName, request.FirstName)
                     .SetProperty(u => u.LastName, request.LastName)
             );

        return Result.Success();
    }

    public async Task<Result> ChangePasswordAsync(string UserId, ChangePasswordVM request)
    {
        var user = await _userManager.FindByIdAsync(UserId);

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        if (result.Succeeded)
            return Result.Success();

        var error = result.Errors.First();

        return Result.Failure(new Error(error.Code, error.Description, ErrorType.Failure));

    }
    public async Task<Result> UpdateProfilePictureAsync(string UserId, IFormFile Picture)
    {
        var pictureUrl = await _cloudinaryService.UploadImageAsync(
            file: Picture,
            folder: "resort-hub/avatars",
            publicId: $"avatar_{UserId}"      
        );

        await _userManager.Users
            .Where(x => x.Id == UserId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.ProfilePictureUrl, pictureUrl)
            );

        return Result.Success();
    }
    public async Task<Result> DeleteProfilePictureAsync(string UserId)
    {
        await _cloudinaryService.DeleteImageAsync($"resort-hub/avatars/avatar_{UserId}");
        string defaultPictureUrl = "/images/DefaultUserImage.png";

        await _userManager.Users
            .Where(x => x.Id == UserId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.ProfilePictureUrl, defaultPictureUrl)
            );

        return Result.Success();
    }
}
