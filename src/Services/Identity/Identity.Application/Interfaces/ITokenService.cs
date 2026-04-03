using Identity.Application.Entities;

namespace Identity.Application.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    string GenerateRefreshToken();
}