using Microsoft.AspNetCore.Http;

namespace IdentityApplication.Common.Interfaces.Services.Identity;

public interface IMediaUpdateService<T>
    where T : class
{
    string? GetKey(IFormFile? avatar);

    Task<string?> UploadAvatarAsync(IFormFile? avatar, string? key);

    Task DeleteAvatarAsync(string? key);
}
