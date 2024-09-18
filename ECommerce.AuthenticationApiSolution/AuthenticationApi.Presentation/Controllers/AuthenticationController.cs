using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[Route("api/[controller]")]
[ApiController]
[AllowAnonymous]
public class AuthenticationController(IUser userService) :ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<Response>> Register(AppUserDto dto)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);
        var result = await userService.RegisterAsync(dto);
        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<Response>> Login(LoginDto dto)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);
        var result = await userService.LoginAsync(dto);
        return result.Flag ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<GetUserDto>> GetUser(int id)
    {
        if(id < 1) return BadRequest("Invalid id");
        var user = await userService.GetUser(id);
        return user.Id > 1 ? Ok(user) : NotFound("No User found");
    }
}