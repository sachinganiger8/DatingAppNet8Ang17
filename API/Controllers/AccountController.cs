using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext dbContext, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await userExists(registerDto.Username)) return BadRequest("Username already taken");

        return Ok();

        // using var hmac = new HMACSHA512();

        // var user = new AppUser
        // {
        //     UserName = registerDto.Username.ToLower(),
        //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        //     PasswordSalt = hmac.Key
        // };

        // await dbContext.Users.AddAsync(user);
        // await dbContext.SaveChangesAsync();
        // return new UserDto{
        //     Username=user.UserName,
        //     Token=tokenService.CreateToken(user)
        // };
    }

    private async Task<bool> userExists(string username)
    {
        return await dbContext.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }

    [Route("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var appUser = await dbContext.Users
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
            Token = tokenService.CreateToken(appUser)
        };
    }
}