using LibraryApi.DTOs;
using Microsoft.AspNetCore.Identity;

namespace LibraryApi.Services.Interfaces;

public interface IAccountService
{
    Task<TokenModel?> LoginAsync(LoginModel model);
    Task<IdentityResult> RegisterAsync(RegisterModel model);
}