using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ZivnostAPI.Models.DatabaseModels.Account;
using ZivnostAPI.Models.JWT;

namespace ZivnostAPI.Services.JWT;

public class JWTService
{
    JwtConfig _jwtConfig;
    public JWTService(JwtConfig jwtConfig)
    {
        _jwtConfig = jwtConfig;
    }

    public string CreateJWTToken(Account account)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, account.Email)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        JwtSecurityToken token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: credentials
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    public bool VerifyPasswordHash(string requestPassword, string dbPasswordHashWithSalt)
    {
        return BCrypt.Net.BCrypt.Verify(requestPassword, dbPasswordHashWithSalt);
    }
}
