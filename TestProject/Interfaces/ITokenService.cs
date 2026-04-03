using TestProject.Entities;

namespace TestProject.Interfaces;

public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
    string GenerateRefreshToken();
}