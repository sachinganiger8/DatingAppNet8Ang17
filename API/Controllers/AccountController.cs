using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext dbContext, ITokenService tokenService,
            IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await userExists(registerDto.Username)) return BadRequest("Username already taken");

        using var hmac = new HMACSHA512();

        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
        user.PasswordSalt = hmac.Key;

        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs
        };
    }

    private async Task<bool> userExists(string username)
    {
        return await dbContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }

    [Route("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var appUser = await dbContext.Users.Include(x => x.Photos)
        .FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

        if (appUser == null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(appUser.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != appUser.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return new UserDto
        {
            Username = appUser.UserName,
            Token = tokenService.CreateToken(appUser),
            PhotoUrl = appUser.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = appUser.KnownAs
        };
    }
}