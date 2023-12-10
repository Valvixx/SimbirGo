using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SimbirGo.Services;

public class Jwt
{
    public static string? GetId(string token)
    {
        return ParserToken(token, "accountId");
    }

    public static string? GetEmail(string token)
    {
        return ParserToken(token, "email");
    }

    public static string? GetUsername(string token)
    {
        return ParserToken(token, "username");
    }

    public static string? GetRole(string token)
    {
        return ParserToken(token, "role");
    }

    private static string? ParserToken(string token, string role)
    {
        var removeBearer = token.Split(' ')[1];
        var handler = new JwtSecurityTokenHandler();
        var tokenData = handler.ReadJwtToken(removeBearer).Payload;
        return tokenData.Claims.FirstOrDefault(c => c.Type.Split('/').Last() == role)?.Value;
    }

    public static List<Claim> GetClaims(int id, string email, string username, string role)
    {
        var claims = new List<Claim>()
        {
            new Claim("accountId", id.ToString()),
            new Claim("email", email),
            new Claim("username", username),
            new Claim("role", role),
        };

        return claims;
    }
}