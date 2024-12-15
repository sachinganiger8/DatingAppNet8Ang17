using System.Security.Cryptography;
using System.Text;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService,
            IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await userExists(registerDto.Username)) return BadRequest("Username already taken");

        var user = mapper.Map<AppUser>(registerDto);
        user.UserName = registerDto.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> userExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }

    [Route("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

        var appUser = await userManager.Users.Include(x => x.Photos)
        .FirstOrDefaultAsync(x => x.NormalizedUserName == loginDto.Username.ToUpper());

        if (appUser == null || appUser.UserName == null) return Unauthorized("Invalid Username");

        var result = await userManager.CheckPasswordAsync(appUser, loginDto.Password);

        if (!result) return Unauthorized();

        return new UserDto
        {
            Username = appUser.UserName,
            Token = await tokenService.CreateToken(appUser),
            PhotoUrl = appUser.Photos.FirstOrDefault(x => x.IsMain)?.Url,
            KnownAs = appUser.KnownAs,
            Gender = appUser.Gender
        };
    }
}