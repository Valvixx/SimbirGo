using System.Security.Claims;
using SimbirGo.Models.Dto;

namespace SimbirGo.Services.Interfaces;

public interface IJwtService
{
    string CreateToken(ICollection<Claim> claims, int tokenExpiresAfterHours = 0);
    Task<bool> CheckAccount(string email);
    Task<AuthDto> ExpireToken(string token);
}