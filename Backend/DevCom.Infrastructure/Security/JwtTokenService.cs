using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DevCom.Application.Interfaces;
using DevCom.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DevCom.Infrastructure.Security;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    public string GenerateToken(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        double hours = double.TryParse(configuration["Jwt:ExpiresInHours"], out var h) ? h : 24;
        var expires  = DateTime.UtcNow.AddHours(hours);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier,     user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name,               user.Name),
            new Claim(ClaimTypes.Role,               user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
        };
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}