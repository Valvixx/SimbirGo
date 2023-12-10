using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using SimbirGo.Models;

namespace SimbirGo.Services;
public interface IAuthorizationService
{
    bool IsValidUser(string username, string password);
}
public class AuthorizationService : IAuthorizationService
{
    private readonly DbConnection _dbConnection;
    private readonly IConfiguration _configuration;

    public AuthorizationService(DbConnection dbConnection, IConfiguration configuration)
    {
        _dbConnection = dbConnection;
        _configuration = configuration;
    }

    public bool IsValidUser(string username, string password)
    {
        const string sql = @"SELECT * FROM accounts WHERE username = @username AND password = @password;";
        var result = _dbConnection.Query<AccountModel>(sql, new { username, password }).FirstOrDefault();

        return result != null;
    }

    public string? GetUserRole(string username)
    {
        const string sql = @"SELECT role FROM accounts WHERE username = @username;";
        
        return _dbConnection.Query<string>(sql, new { username }).FirstOrDefault();
    }
    
    public string GenerateToken(IEnumerable<Claim> authClaims)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtOptions:SecretKey"] ?? string.Empty));
        _ = int.TryParse(_configuration["JwtOptions:TokenExpiresAfterHours"], out var tokenValidityInHours);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtOptions:Issuer"],
            audience: _configuration["JwtOptions:Audience"],
            expires: DateTime.Now.AddHours(tokenValidityInHours),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}