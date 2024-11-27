using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Servicess;

public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser appUser)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("cannot access tokenkey from appsettings");
        if (tokenKey.Length < 64) throw new Exception("your token keys needs to be longer");

        if (appUser.UserName == null) throw new Exception("username not exists for the user");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        var claims = new List<Claim>(){
            new(ClaimTypes.NameIdentifier,appUser.Id.ToString()),
            new(ClaimTypes.Name,appUser.UserName)
        };

        var roles = await userManager.GetRolesAsync(appUser);

        claims.AddRange(roles.Select(s => new Claim(ClaimTypes.Role, s)));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
