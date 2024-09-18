using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationApi.Infrastructure.Repositories;

public class UserRepository(AuthenticationDbContext dbContext, IConfiguration config) : IUser
{
    private async Task<AppUser> GetUserByEmail(string email)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        return user is null ? null! : user!;
    }

    public async Task<Response> RegisterAsync(AppUserDto dto)
    {
        var user = await GetUserByEmail(dto.Email);
        if (user is not null)
        {
            return new Response(false, $"Email {dto.Email} already exists.");
        }

        var result = dbContext.Users.Add(new AppUser()
        {
            Name = dto.Name,
            Address = dto.Address,
            TelephoneNumber = dto.TelephoneNumber,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role,
        });
        await dbContext.SaveChangesAsync();
        return result.Entity.Id > 0
            ? new Response(true, "User registered successfully")
            : new Response(false, "Failed to register user.");
    }

    public async Task<Response> LoginAsync(LoginDto dto)
    {
        var user = await GetUserByEmail(dto.Email);
        if (user is null)
        {
            return new Response(false, $"Email {dto.Email} does not exists.");
        }

        bool verifyPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);
        if (!verifyPassword)
        {
            return new Response(false, $"Email {dto.Email} does not match password.");
        }

        return new Response(true, GenerateToken(user));
    }

    private string GenerateToken(AppUser user)
    {
        var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
        var securityKey = new SymmetricSecurityKey(key);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Name!),
            new(ClaimTypes.Email, user.Email!)
        };

        if (!string.IsNullOrEmpty(user.Role) || !Equals("string", user.Role))
        {
            claims.Add(new(ClaimTypes.Role, user.Role!));
        }

        return new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
            issuer: config.GetSection("Authentication:Issuer").Value!,
            audience: config.GetSection("Authentication:Audience").Value!,
            claims: claims,
            expires: null,
            signingCredentials: credentials
        ));
    }

    public async Task<GetUserDto> GetUser(int userId)
    {
        var user = await dbContext.Users.FindAsync(userId);
        return user is null
            ? null!
            : new GetUserDto(
                user.Id,
                user.Name!,
                user.TelephoneNumber!,
                user.Address!,
                user.Email!,
                user.Role!
            );
    }
}