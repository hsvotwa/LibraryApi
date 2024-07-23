using LibraryApi.DTOs;
using LibraryApi.Entities;
using LibraryApi.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryApi.Services.Implementations;

public class AccountService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<IdentityResult> RegisterAsync(RegisterModel model)
    {
        ApplicationUser user = new()
        {
            UserName = model.Email,
            Email = model.Email
        };
        IdentityResult result = await _userManager.CreateAsync(user, model.Password);
        return result;
    }

    public async Task<TokenModel?> LoginAsync(LoginModel model)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            Claim[] authClaims =
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ];

            SymmetricSecurityKey authSigningKey = new(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!));
            var expiry = DateTime.Now.AddHours(Convert.ToDouble(_configuration["Jwt:TokenExpiryInHours"]));

            JwtSecurityToken token = new(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                expires: expiry,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenModel
            {
                Token = tokenStr,
                Expiry = expiry
            };
        }
        return null;
    }
}